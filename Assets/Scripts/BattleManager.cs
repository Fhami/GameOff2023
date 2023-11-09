using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public enum GameEvent
    {
        NONE,
        CARD_DRAWN,
        CARD_DISCARDED,
        CARD_DESTROYED,
        CARD_FADED,
        CARD_PLAYED,
        PLAYER_TURN_START,
        PLAYER_TURN_END
    }
    
    /// <summary>
    /// Main manager used in BattleScene.
    /// </summary>
    public class BattleManager : MonoBehaviour
    {
        private IEnumerator Start()
        {
            // TODO: Initialize battle scene and start the battle!
            yield break;
        }

        // TODO: This should be called when player turn starts before player can play cards
        public IEnumerator PlayerTurnStart(RuntimeCharacter player)
        {
            FormData form = player.GetCurrentForm();

            // Clear shield stack TODO: If there's an artifact e.g. "Don't clear shield at turn start" we can do it here.
            player.properties.Get<int>(PropertyKey.SHIELD).Value = 0;

            // Reset action points to the base action point value of their current form // TODO: Can add a modifier here if we want
            player.properties.Get<int>(PropertyKey.ACTION_POINTS).Value = form.actionPoints;
            
            // TODO: Execute PLAYER_TURN_START skills/effects
            
            // TODO: Draw cards based on player action point value?
            
            // Clear properties that are only tracked per turn
            player.properties.Get<int>(PropertyKey.FORM_CHANGED_COUNT_CURRENT_TURN).Value = 0;
            player.properties.Get<bool>(PropertyKey.CANNOT_DRAW_ADDITIONAL_CARDS_CURRENT_TURN).Value = false;
            player.properties.Get<int>(PropertyKey.EVADE).Value = 0;
            player.properties.Get<int>(PropertyKey.STUN).Value = 0;

            // If player is stunned don't allow them to play any cards
            if (player.properties.Get<int>(PropertyKey.STUN).Value > 0)
            {
                // TODO: Don't allow playing cards
                throw new NotImplementedException();
            }
            
            yield break;
        }

        // TODO: This should be called when player turn ends before we start the enemy turn
        public IEnumerator PlayerTurnEnd(RuntimeCharacter player)
        {
            FormData form = player.GetCurrentForm();
            
            // TODO: Execute PLAYER_TURN_END skills/effects
            
            Property<int> power = player.properties.Get<int>(PropertyKey.POWER);
            Property<int> maxPower = player.properties.Get<int>(PropertyKey.MAX_POWER);
            
            // TODO: Handle overload logic and overload effects. QUESTION: Do we allow power go over max power or do we cap it at max power?
            if (power.Value >= maxPower.Value)
            {
                throw new NotImplementedException();
            }
            if (power.Value <= 0)
            {
                // TODO: Handle player death when they turn into dust
                throw new NotImplementedException();
            }

            // Clear stun (it's not stackable right?)
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
            card.cardState = CardState.PLAYING;
            
            // TODO: Execute passive abilities that trigger on CardEvent.CARD_PLAYED (I guess we do this here before executing this card's effects?)

            // Execute card effects one by one
            foreach (EffectData effectData in card.cardData.effects)
            {
                yield return effectData.Execute(card, player, player, target, enemies);

                // Exit early if the card was FADED or DESTROYED (so we don't try to execute effects on invalid card)
                if (card.cardState is CardState.FADED or CardState.DESTROYED)
                {
                    break;
                }
            }

            // If the card is not FADED or DESTROYED then we can move it to discard pile
            if (card.cardState != CardState.FADED && card.cardState != CardState.DESTROYED)
            {
                yield return DiscardCard(card, player);
            }
        }
        
        public IEnumerator DiscardCard(RuntimeCard card, RuntimeCharacter player)
        {
            // TODO: Discard the card (visual + data)
            
            card.cardState = CardState.DISCARD_PILE;
            
            player.properties.Get<int>(PropertyKey.CARDS_DISCARDED_ON_CURRENT_TURN_COUNT).Value++;
            player.properties.Get<int>(PropertyKey.CARDS_DISCARDED_ON_CURRENT_BATTLE_COUNT).Value++;

            // TODO: Execute passive abilities that trigger on CardEvent.CARD_DISCARDED
            
            throw new System.NotImplementedException();
        }
        
        /// <summary>
        /// Coroutine that should be called before enemy acts their intent.
        /// </summary>
        /// <param name="enemy">The enemy whose turn it is now.</param>
        public IEnumerator EnemyTurnStart(RuntimeCharacter enemy)
        {
            FormData form = enemy.GetCurrentForm();

            // Clear shield stack TODO: If there's an artifact e.g. "Don't clear shield at turn start" we can do it here.
            enemy.properties.Get<int>(PropertyKey.SHIELD).Value = 0;

            // Reset action points to the base action point value of their current form // TODO: Can add a modifier here if we want
            // NOTE: I think enemies don't need AP since they use attack patterns
            enemy.properties.Get<int>(PropertyKey.ACTION_POINTS).Value = form.actionPoints;
          
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
            Property<int> power = enemy.properties.Get<int>(PropertyKey.POWER);
            Property<int> maxPower = enemy.properties.Get<int>(PropertyKey.MAX_POWER);
            
            // TODO: Handle overload logic and overload effects. QUESTION: Do we allow power go over max power or do we cap it at max power?
            if (power.Value >= maxPower.Value)
            {
                throw new NotImplementedException();
            }
            if (power.Value <= 0)
            {
                // TODO: Handle enemy death when they turn into dust
                throw new NotImplementedException();
            }
            
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
    }
}

