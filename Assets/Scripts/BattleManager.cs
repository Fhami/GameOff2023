using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public enum GameEvent
    {
        NONE,
        ON_CARD_DRAWN,
        ON_CARD_DISCARDED,
        ON_CARD_DESTROYED,
        ON_CARD_FADED,
        ON_CARD_PLAYED,
        ON_PLAYER_TURN_START,
        ON_PLAYER_TURN_END,
        ON_SIZE_CHANGED,
        ON_HEALTH_CHANGED,
        ON_FORM_CHANGED,
        ON_DEATH,
        ON_BATTLE_START
    }
    
    /// <summary>
    /// Main manager used in BattleScene.
    /// </summary>
    public class BattleManager : MonoBehaviour
    {
        //Only player can play cards so we put this here
        [SerializeField] private CardController cardController;
        
        public Character player;
        public RuntimeCharacter runtimePlayer;
        public List<Character> enemies = new List<Character>();
        public List<RuntimeCharacter> runtimeEnemies = new List<RuntimeCharacter>();

        [Header("Mockup")] 
        public DeckData deckData;

        //Static instance for easy access, this won't be singleton cuz we only need it in battle scene
        public static BattleManager current;

        private bool isDebug = true;
        private void Awake()
        {
            current = this;
        }

        public void EndTurn()
        {
            StartCoroutine(IEEndTurn());
        }
        
        public IEnumerator IEEndTurn()
        {
            yield return PlayerTurnEnd(runtimePlayer, runtimeEnemies);
            
            //Play Enemies turn
            foreach (var _enemy in runtimeEnemies)
            {
                yield return EnemyTurnStart(_enemy);
                
                yield return PlayEnemyTurn(_enemy, runtimePlayer, runtimeEnemies);

                yield return EnemyTurnEnd(_enemy);
            }

            yield return PlayerTurnStart(runtimePlayer, runtimeEnemies);
        }
        
        private IEnumerator Start()
        {
            if (isDebug)
            {
                Database.Initialize();
                //Add cards to player deck
                foreach (var _cardData in deckData.Cards)
                {
                    GameManager.Instance.PlayerRuntimeDeck.AddCard(CardFactory.Create(_cardData.name));
                }
                
                foreach (var _card in GameManager.Instance.PlayerRuntimeDeck.Cards)
                {
                    //Create card object
                    var _newCardObj = CardFactory.CreateCardObject(_card);
                
                    cardController.Deck.AddCard(_newCardObj);
                }
                
                player = CharacterFactory.CreateCharacterObject("Muscle Mage");
                player.gameObject.tag = "PLAYER";

                cardController.Character = player;

                runtimePlayer = player.runtimeCharacter;

                var _enemy = CharacterFactory.CreateCharacterObject("Fishy");
                _enemy.gameObject.tag = "ENEMY";
                
                enemies.Add(_enemy);
                runtimeEnemies.Add(_enemy.runtimeCharacter);
            }
            
            // TODO: Initialize battle scene and start the battle!
            yield return BattleStart(runtimePlayer, runtimeEnemies);

            yield return PlayerTurnStart(runtimePlayer, runtimeEnemies);
        }
        
        public IEnumerator BattleStart(RuntimeCharacter player, List<RuntimeCharacter> enemies)
        {
            // Set each skill to ready state.
            foreach (RuntimeSkill skill in player.skills)
            {
                skill.properties.Get<SkillState>(PropertyKey.SKILL_STATE).Value = SkillState.READY;
            }
            
            yield return OnGameEvent(GameEvent.ON_BATTLE_START, player, player, enemies);
        }
        
        // TODO: This should be called when player turn starts before player can play cards
        public IEnumerator PlayerTurnStart(RuntimeCharacter player, List<RuntimeCharacter> enemies)
        {
            FormData form = player.GetCurrentForm();

            // Clear shield stack TODO: If there's an artifact e.g. "Don't clear shield at turn start" we can do it here.
            player.properties.Get<int>(PropertyKey.SHIELD).Value = 0;

            // Set character's hand size to match the form hand size
            // TODO: We could have modifiers (e.g. artifacts) which modify the base hand size value
            player.properties.Get<int>(PropertyKey.HAND_SIZE).Value = form.handSize;
            
            // TODO: Draw cards based on player action point value?
            
            // Clear properties that are only tracked per turn
            player.properties.Get<int>(PropertyKey.FORM_CHANGED_COUNT_CURRENT_TURN).Value = 0;
            player.properties.Get<bool>(PropertyKey.CANNOT_DRAW_ADDITIONAL_CARDS_CURRENT_TURN).Value = false;
            player.properties.Get<int>(PropertyKey.EVADE).Value = 0;
            player.properties.Get<int>(PropertyKey.STUN).Value = 0;

            // If player is stunned don't allow them to play any cards
            if (player.properties.Get<int>(PropertyKey.STUN).Value > 0)
            {
                // TODO: Don't allow playing cards / do we need STUN feedback for player?
                throw new NotImplementedException();
            }
          
            // Draw cards based on the hand size
            int handSize = player.properties.Get<int>(PropertyKey.HAND_SIZE).GetValueWithModifiers(player);
            for (int i = 0; i < handSize; i++)
            {
                yield return DrawCard(player, enemies);
            }

            yield return OnGameEvent(GameEvent.ON_PLAYER_TURN_START, player, player, enemies);
        }

        // TODO: This should be called when player turn ends before we start the enemy turn
        public IEnumerator PlayerTurnEnd(RuntimeCharacter player, List<RuntimeCharacter> enemies)
        {
            // TODO: Discard all remaining cards in your hand to the discard pile

            yield return cardController.ClearHand();
            
            yield return OnGameEvent(GameEvent.ON_PLAYER_TURN_END, player, player, enemies);
            
            // Clear buff stacks
            player.properties.Get<int>(PropertyKey.STUN).Value = 0;
            
            // Clear properties that are only tracked per turn
            player.properties.Get<int>(PropertyKey.CARDS_DISCARDED_ON_CURRENT_TURN_COUNT).Value = 0;
            player.properties.Get<int>(PropertyKey.CARDS_DESTROYED_ON_CURRENT_TURN_COUNT).Value = 0;
            player.properties.Get<int>(PropertyKey.CARDS_FADED_ON_CURRENT_TURN_COUNT).Value = 0;

            yield break;
        }

        /// <summary>
        /// A coroutine that handles the logic for playing a card on player's turn. Enemies use another coroutine (<see cref="PlayEnemyTurn"/>).
        /// </summary>
        /// <param name="card">The card the player is going to play.</param>
        /// <param name="player">The player character.</param>
        /// <param name="target">The character the player drag &amp; drops the card on top of. This can be either player or enemy.</param>
        /// <param name="enemies">The list of all enemies in the current battle.</param>
        public IEnumerator PlayCard(RuntimeCard card, RuntimeCharacter player, RuntimeCharacter target, List<RuntimeCharacter> enemies)
        {
            card.properties.Get<CardState>(PropertyKey.CARD_STATE).Value = CardState.PLAYING;
            
            yield return OnGameEvent(GameEvent.ON_CARD_PLAYED, player, player, enemies); // NOTE: Not sure if this should happen here or below after executing the card effects?

            // Execute card effects one by one
            foreach (EffectData effectData in card.cardData.effects)
            {
                yield return effectData.Execute(card, player, player, target, enemies);

                // Exit early if the card was FADED or DESTROYED (so we don't try to execute effects on invalid card)
                if (card.properties.Get<CardState>(PropertyKey.CARD_STATE).Value is CardState.FADED or CardState.DESTROYED)
                {
                    break;
                }
            }
            
            // If the card is not FADED or DESTROYED then we can move it to discard pile
            if (card.properties.Get<CardState>(PropertyKey.CARD_STATE).Value != CardState.FADED &&
                card.properties.Get<CardState>(PropertyKey.CARD_STATE).Value != CardState.DESTROYED)
            {
                yield return DiscardCard(card, player, enemies);
            }
        }
        
        public IEnumerator DiscardCard(RuntimeCard card, RuntimeCharacter player, List<RuntimeCharacter> enemies)
        {
            // TODO: Discard the card (visual + data)
            
            card.properties.Get<CardState>(PropertyKey.CARD_STATE).Value = CardState.DISCARD_PILE;
            
            player.properties.Get<int>(PropertyKey.CARDS_DISCARDED_ON_CURRENT_TURN_COUNT).Value++;
            player.properties.Get<int>(PropertyKey.CARDS_DISCARDED_ON_CURRENT_BATTLE_COUNT).Value++;

            yield return cardController.Discard(card.Card);

            yield return OnGameEvent(GameEvent.ON_CARD_DISCARDED, player, player, enemies);
        }
        
        public IEnumerator DrawCard(RuntimeCharacter player, List<RuntimeCharacter> enemies)
        {
            // TODO: Draw the card (visual + data)
            yield return cardController.Draw(1);
            
            yield return OnGameEvent(GameEvent.ON_CARD_DRAWN, player, player, enemies);
        }
        
        /// <summary>
        /// Coroutine that should be called before enemy acts their intent.
        /// </summary>
        /// <param name="enemy">The enemy whose turn it is now.</param>
        public IEnumerator EnemyTurnStart(RuntimeCharacter enemy)
        {
            // Clear shield stack TODO: If there's an artifact e.g. "Don't clear shield at turn start" we can do it here.
            enemy.properties.Get<int>(PropertyKey.SHIELD).Value = 0;
          
            // Clear properties that are only tracked per turn
            enemy.properties.Get<int>(PropertyKey.FORM_CHANGED_COUNT_CURRENT_TURN).Value = 0;
            enemy.properties.Get<bool>(PropertyKey.CANNOT_DRAW_ADDITIONAL_CARDS_CURRENT_TURN).Value = false;
            enemy.properties.Get<int>(PropertyKey.EVADE).Value = 0;
            
            yield break;
        }
        
        /// <summary>
        /// Coroutine that should be called after enemy finished acting.
        /// </summary>
        /// <param name="enemy">The enemy whose turn just ended.</param>
        public IEnumerator EnemyTurnEnd(RuntimeCharacter enemy)
        {
            // Clear stun (it's not stackable right?)
            enemy.properties.Get<int>(PropertyKey.STUN).Value = 0;
            
            // Clear properties that are only tracked per turn
            enemy.properties.Get<int>(PropertyKey.CARDS_DISCARDED_ON_CURRENT_TURN_COUNT).Value = 0;
            enemy.properties.Get<int>(PropertyKey.CARDS_DESTROYED_ON_CURRENT_TURN_COUNT).Value = 0;
            enemy.properties.Get<int>(PropertyKey.CARDS_FADED_ON_CURRENT_TURN_COUNT).Value = 0;
            
            yield break;
        }

        /// <summary>
        /// Coroutine that executes a single enemy turn.
        /// </summary>
        /// <param name="enemy">The enemy whose turn we execute.</param>
        /// <param name="player">The (human) player.</param>
        /// <param name="enemies">The list of all enemies in the current battle.</param>
        /// <returns></returns>
        public IEnumerator PlayEnemyTurn(RuntimeCharacter enemy, RuntimeCharacter player, List<RuntimeCharacter> enemies)
        {
            // If enemy is stunned don't allow them to play any cards
            if (enemy.properties.Get<int>(PropertyKey.STUN).Value > 0)
            {
                // TODO: Skip enemy turn logic / VFX? / animation?
                throw new NotImplementedException();
            }
           
            FormData form = enemy.GetCurrentForm();

            // Get enemy's next intent (the next card they plan to use)
            Property<int> cardIndex = enemy.properties.Get<int>(PropertyKey.ENEMY_ATTACK_PATTERN_CARD_INDEX);

            CardData cardData = form.attackPattern[cardIndex.Value];

            // Create card instance from the card data
            // TODO: I'm not sure what ID we should give to the factory to create a new card instance (or should we just give reference to CardData)
            RuntimeCard card = CardFactory.Create(cardData.name);

            // NOTE: I think for enemy we don't care about FADED, DESTROYED etc. since their cards probably don't use any of those
            // so just execute the effects
            foreach (EffectData effectData in card.cardData.effects)
            {
                yield return effectData.Execute(card, enemy, player, player, enemies);
            }
        
            // Increment the index by 1 (wrap back to 0 if needed)
            cardIndex.Value = (cardIndex.Value + 1) % form.attackPattern.Count;
        }
        
        // TODO: Call this after current battle ended
        public void ClearCurrentBattleProperties(RuntimeCharacter player)
        {
            player.properties.Get<int>(PropertyKey.CARDS_DISCARDED_ON_CURRENT_BATTLE_COUNT).Value = 0;
            player.properties.Get<int>(PropertyKey.CARDS_DESTROYED_ON_CURRENT_BATTLE_COUNT).Value = 0;
            player.properties.Get<int>(PropertyKey.CARDS_FADED_ON_CURRENT_BATTLE_COUNT).Value = 0;
        }

        public static IEnumerator OnGameEvent(GameEvent gameEvent, RuntimeCharacter character, RuntimeCharacter player, List<RuntimeCharacter> enemies)
        {
            yield return TryRechargeActiveSkills(gameEvent, character, player, enemies);
            yield return TryTriggerActiveSkills(gameEvent, character, player, enemies);
        }

        public static IEnumerator TryTriggerActiveSkills(GameEvent gameEvent, RuntimeCharacter character, RuntimeCharacter player, List<RuntimeCharacter> enemies)
        {
            // Loop through character's active skills and see if any of them trigger.
            foreach (RuntimeSkill skill in character.skills)
            {
                if (TriggerActiveSkill(gameEvent, skill, character, player))
                {
                    // Set the active skill as USED. It won't trigger again unless it gets recharged.
                    skill.properties.Get<SkillState>(PropertyKey.SKILL_STATE).Value = SkillState.USED;

                    // Create an instance of a card and execute it's effects
                    RuntimeCard card = CardFactory.Create(skill.skillData.card.name);
                    
                    foreach (EffectData effectData in skill.skillData.card.effects)
                    {
                        yield return effectData.Execute(card, character, player, player, enemies);
                    }
                }
            }
        }

        public static bool TriggerActiveSkill(GameEvent gameEvent, RuntimeSkill skill, RuntimeCharacter character, RuntimeCharacter player)
        {
            // The current game event must match the skill's trigger game event.
            if (skill.skillData.triggerGameEvent != gameEvent)
            {
                return false;
            }
            
            // Only skills that are READY can trigger.
            if (skill.properties.Get<SkillState>(PropertyKey.SKILL_STATE).Value != SkillState.READY)
            {
                return false;
            }

            if (skill.skillData.triggerConditions != null)
            {
                foreach (ConditionData condition in skill.skillData.triggerConditions)
                {
                    // If any condition fails the skill won't trigger.
                    if (!condition.Evaluate(gameEvent, character, player))
                    {
                        return false;
                    }
                }
                
                // All conditions passed and the skill will trigger!
                return true;
            }

            // There's no trigger conditions assigned, therefore the skill can't trigger. :(
            return false;
        }
        
        public static IEnumerator TryRechargeActiveSkills(GameEvent gameEvent, RuntimeCharacter character, RuntimeCharacter player, List<RuntimeCharacter> enemies)
        {
            // Loop through character's active skills and see if any of them trigger.
            foreach (RuntimeSkill skill in character.skills)
            {
                if (RechargeActiveSkill(gameEvent, skill, character, player))
                {
                    // TODO: VFX, animation etc.
                    // Set the active skill as USED. It won't trigger again unless it gets recharged.
                    skill.properties.Get<SkillState>(PropertyKey.SKILL_STATE).Value = SkillState.READY;
                }
            }
            
            yield break;
        }
        
        public static bool RechargeActiveSkill(GameEvent gameEvent, RuntimeSkill skill, RuntimeCharacter character, RuntimeCharacter player)
        {
            // The current game event must match the skill's recharge game event.
            if (skill.skillData.rechargeGameEvent != gameEvent)
            {
                return false;
            }
            
            // Only skills that are USED can recharge.
            if (skill.properties.Get<SkillState>(PropertyKey.SKILL_STATE).Value != SkillState.USED)
            {
                return false;
            }

            if (skill.skillData.rechargeConditions != null)
            {
                foreach (ConditionData condition in skill.skillData.triggerConditions)
                {
                    // If any condition fails the skill won't recharge.
                    if (!condition.Evaluate(gameEvent, character, player))
                    {
                        return false;
                    }
                }
                
                // All conditions passed and the skill will recharge!
                return true;
            }

            // There's no recharge conditions assigned, therefore the skill can't recharge. :(
            return false;
        }
    }
}

