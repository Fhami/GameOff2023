using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
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
        ON_CARD_SHUFFLED,
        ON_PLAYER_TURN_START,
        ON_PLAYER_TURN_END,
        ON_SIZE_CHANGED,
        ON_HEALTH_CHANGED,
        ON_FORM_CHANGED,
        ON_DEATH,
        ON_CHARACTER_SPAWNED,
        ON_BATTLE_START,
        ON_PLAYER_SIZE_CHANGED,
        ON_PLAYER_USE_SKILL
    }
    
    /// <summary>
    /// Main manager used in BattleScene.
    /// </summary>
    public class BattleManager : MonoBehaviour
    {
        public CardController CardController => cardController;

        public TooltipUI TooltipUI;
        
        //Only player can play cards so we put this here
        [SerializeField] private CardController cardController;
        [SerializeField] private CharacterSpawner characterSpawner;
        [SerializeField] private float characterSpawnDelay = 0.2f;
        [SerializeField] private float stunDuration = 0.3f;

        public Character player;
        public RuntimeCharacter runtimePlayer;
        public List<Character> enemies = new List<Character>();
        public List<RuntimeCharacter> runtimeEnemies = new List<RuntimeCharacter>();

        [Header("Mockup")] 
        public DeckData deckData;
        public CharacterData playerData;
        public EncounterData encounterData;

        [Foldout("Sound")] 
        public AudioClip bgm;
        //public AudioClip 

        //Static instance for easy access, this won't be singleton cuz we only need it in battle scene
        public static BattleManager current;
        //Use this to prevent player playing card while redraw
        public bool canPlayCard = true;

        private bool isDebug = true;
        private void Awake()
        {
            current = this;
        }
        
        /// <summary>
        /// This is for testing! Actual start is StartBattle
        /// </summary>
        /// <returns></returns>
        private IEnumerator Start()
        {
            if (!isDebug) yield break;
            
            Database.Initialize();
            //Add cards to player deck
            foreach (var _cardData in deckData.Cards)
            {
                GameManager.Instance.PlayerRuntimeDeck.AddCard(CardFactory.Create(_cardData.name));
            }

            yield return StartBattle(playerData, encounterData);
        }

        public IEnumerator StartBattle(CharacterData _playerData, EncounterData _encounterData)
        {
            SoundManager.Instance.PlayBGM(bgm);
            
            canPlayCard = true;
            
            yield return cardController.InitializeDeck(GameManager.Instance.PlayerRuntimeDeck);

            yield return InitializeCharacters(_playerData, _encounterData);
            
            yield return BattleStart(runtimePlayer, runtimeEnemies);

            yield return PlayerTurnStart(runtimePlayer, runtimeEnemies);
        }

        public IEnumerator InitializeCharacters(CharacterData _player, EncounterData _encounterData)
        {
            player = characterSpawner.SpawnPlayer(_player.name);
            player.cardController = cardController;
            
            cardController.Character = player;
            runtimePlayer = player.runtimeCharacter;

            if (_encounterData != null) // I added this null check so I can test cards without having any
            {
                foreach (var _enemyData in _encounterData.enemies)
                {
                    var _newEnemy = characterSpawner.SpawnEnemy(_enemyData);
                
                    enemies.Add(_newEnemy);
                    runtimeEnemies.Add(_newEnemy.runtimeCharacter);
                    
                    yield return _newEnemy.runtimeCharacter.Character.UpdateIntention(_newEnemy.GetIntention());
                
                    yield return new WaitForSeconds(characterSpawnDelay);

                    //Should we also call GameEvent.ON_CHARACTER_SPAWNED when Initialize?
                    //I don't think we're gonna trigger effect while initializing
                }
            }
        }
        
        public IEnumerator SpawnEnemy(string _name)
        {
            var _newEnemy = characterSpawner.SpawnEnemy(_name);

            yield return new WaitForSeconds(characterSpawnDelay);

            yield return OnGameEvent(GameEvent.ON_CHARACTER_SPAWNED, _newEnemy.runtimeCharacter, runtimePlayer, runtimeEnemies);
        }

        public IEnumerator FleeFromBattle(RuntimeCharacter runtimeCharacter)
        {
            runtimeCharacter.properties.Get<CharacterState>(PropertyKey.CHARACTER_STATE).Value = CharacterState.ESCAPED;
            
            throw new NotImplementedException("TODO: Implement enemy flee logic");
        }
        
        //Bind with button
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
        
        public IEnumerator BattleStart(RuntimeCharacter player, List<RuntimeCharacter> enemies)
        {
            // Set each skill to ready state.
            foreach (RuntimeSkill skill in player.skills)
            {
                skill.properties.Get<SkillState>(PropertyKey.SKILL_STATE).Value = SkillState.READY;
            }
            
            // Enable passives for each character based on their current form
            player.EnablePassives(player.GetCurrentForm());
            foreach (RuntimeCharacter enemy in enemies)
            {
                enemy.EnablePassives(enemy.GetCurrentForm());
            }
            
            yield return OnGameEvent(GameEvent.ON_BATTLE_START, player, player, enemies);
        }
        
        //This should be called when player turn starts before player can play cards
        public IEnumerator PlayerTurnStart(RuntimeCharacter player, List<RuntimeCharacter> enemies)
        {
            FormData form = player.GetCurrentForm();

            // Clear shield stack TODO: If there's an artifact e.g. "Don't clear shield at turn start" we can do it here.
            player.properties.Get<int>(PropertyKey.SHIELD).Value = 0;

            // Set character's hand size to match the form hand size
            // TODO: We could have modifiers (e.g. artifacts) which modify the base hand size value
            player.properties.Get<int>(PropertyKey.HAND_SIZE).Value = player.properties.Get<int>(PropertyKey.HAND_SIZE).GetValueWithModifiers(player);

            // Clear properties that are only tracked per turn
            player.properties.Get<int>(PropertyKey.FORM_CHANGED_COUNT_CURRENT_TURN).Value = 0;
            player.properties.Get<bool>(PropertyKey.CANNOT_DRAW_ADDITIONAL_CARDS_CURRENT_TURN).Value = false;
            player.properties.Get<int>(PropertyKey.EVASION).Value = 0;
            player.properties.Get<int>(PropertyKey.STUN).Value = 0;

            // If player is stunned don't allow them to play any cards
            if (player.properties.Get<int>(PropertyKey.STUN).Value > 0)
            {
                player.Character.PlayParticle(FXKey.STUN);
                
                //Wait for particle to play for a while before skip
                yield return new WaitForSeconds(stunDuration);
                
            }
            else
            {
                // Draw cards based on the hand size
                int handSize = player.properties.Get<int>(PropertyKey.HAND_SIZE).GetValueWithModifiers(player);
                for (int i = 0; i < handSize; i++)
                {
                    yield return DrawCard(null, player, player, null, enemies);
                }
            }

            yield return OnGameEvent(GameEvent.ON_PLAYER_TURN_START, player, player, enemies);
        }

        //This should be called when player turn ends before we start the enemy turn
        public IEnumerator PlayerTurnEnd(RuntimeCharacter player, List<RuntimeCharacter> enemies)
        {
            // Discard hand but since this discard is not initiated by a card we leave it null.
            yield return DiscardHand(null);

            // If player has decay debuff
            if (player.properties.Get<int>(PropertyKey.DECAY).GetValueWithModifiers(player) > 0)
            {
                yield return Decay(player, player, enemies);
            }
            
            // If player has grow buff
            if (player.properties.Get<int>(PropertyKey.GROW).GetValueWithModifiers(player) > 0)
            {
                yield return Grow(player, player, enemies);
            }
            
            yield return OnGameEvent(GameEvent.ON_PLAYER_TURN_END, player, player, enemies);
            
            // Clear buff stacks
            player.properties.Get<int>(PropertyKey.STUN).Value = 0;
            player.properties.Get<int>(PropertyKey.THORNS).Value = 0;

            // Clear properties that are only tracked per turn
            player.properties.Get<int>(PropertyKey.CARDS_DISCARDED_ON_CURRENT_TURN_COUNT).Value = 0;
            player.properties.Get<int>(PropertyKey.CARDS_DESTROYED_ON_CURRENT_TURN_COUNT).Value = 0;
            player.properties.Get<int>(PropertyKey.CARDS_FADED_ON_CURRENT_TURN_COUNT).Value = 0;

            // Death by size (if player is at min size or max size at the end of the turn they will die)
            int minSize = player.properties.Get<int>(PropertyKey.MIN_SIZE).Value;
            int maxSize = player.properties.Get<int>(PropertyKey.MAX_SIZE).Value;
            int size = player.properties.Get<int>(PropertyKey.SIZE).Value;
            
            if (size == minSize)
            {
                yield return Kill(player, null, player, null, enemies, FXKey.SMALL_DEATH);
            }
            else if (size == maxSize)
            {
                yield return Kill(player, null, player, null, enemies, FXKey.BIG_DEATH);
            }
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

            var extraPlayTimes = player.properties.Get<int>(PropertyKey.NEXT_CARD_PLAY_EXTRA_TIMES);

            while (extraPlayTimes.Value > 0)
            {
                var clonedCard = CardFactory.CloneCard(card);

                foreach (var effectData in clonedCard.cardData.effects)
                {
                    yield return effectData.Execute(clonedCard, player, player, target, enemies);
                }
                
                extraPlayTimes.Value--;
            }
            
            // Execute card effects one by one
            foreach (EffectData effectData in card.cardData.effects)
            {
                yield return effectData.Execute(card, player, player, target, enemies);
            }
            
            // If the card is not FADED, DESTROYED or DISCARDED then we can move it to discard pile
            // NOTE: Some card effect might discard the card before we get here! 
            if (card.properties.Get<CardState>(PropertyKey.CARD_STATE).Value != CardState.FADED &&
                card.properties.Get<CardState>(PropertyKey.CARD_STATE).Value != CardState.DESTROYED &&
                card.properties.Get<CardState>(PropertyKey.CARD_STATE).Value != CardState.DISCARD_PILE)
            {
                yield return DiscardCard(card, player, player, target, enemies);
            }
            
            // This is where we finished playing the card, so all effects are executed. Now we can
            // safely (I think) reset some properties used by subsequent effects. Like this one.
            player.properties.Get<int>(PropertyKey.CARDS_DISCARDED_BY_CURRENTLY_BEING_PLAYED_CARD).Value = 0;
        }

        // public IEnumerator PlayActiveSkill(RuntimeCard card, RuntimeCharacter player, RuntimeCharacter target,
        //     List<RuntimeCharacter> enemies)
        // {
        //     
        // }

        public IEnumerator ExhaustCard(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            // Update card state
            card.properties.Get<CardState>(PropertyKey.CARD_STATE).Value = CardState.FADED;
            
            // Update fade stats
            characterPlayingTheCard.properties.Get<int>(PropertyKey.CARDS_FADED_ON_CURRENT_TURN_COUNT).Value++;
            characterPlayingTheCard.properties.Get<int>(PropertyKey.CARDS_FADED_ON_CURRENT_BATTLE_COUNT).Value++;
            
            // Handle visuals
            yield return cardController.ExhaustCard(card.Card);
            
            // Handle game event (skills etc. can trigger here)
            yield return OnGameEvent(GameEvent.ON_CARD_FADED, characterPlayingTheCard, player, enemies);
        }
        
        public IEnumerator DestroyCard(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            // Update card state
            card.properties.Get<CardState>(PropertyKey.CARD_STATE).Value = CardState.DESTROYED;
            
            // Update destroyed stats
            characterPlayingTheCard.properties.Get<int>(PropertyKey.CARDS_DESTROYED_ON_CURRENT_TURN_COUNT).Value++;
            characterPlayingTheCard.properties.Get<int>(PropertyKey.CARDS_DESTROYED_ON_CURRENT_BATTLE_COUNT).Value++;
            
            // Handle visuals
            yield return cardController.DestroyCard(card.Card);
            
            // Handle game event (skills etc. can trigger here)
            yield return OnGameEvent(GameEvent.ON_CARD_DESTROYED, characterPlayingTheCard, player, enemies);
        }

        /// <summary>
        /// Discard all cards in hand.
        /// </summary>
        /// <param name="card">The card which initiated the discard hand logic (as an effect).</param>
        /// <returns></returns>
        public IEnumerator DiscardHand(RuntimeCard card)
        {
            if (card != null)
            {
                // Update this we can use it for effects like (discard your hand, then deal damage equal to how many cards discarded).
                // This will reset to 0 after the card "finishes playing".
                Property<int> cardsDiscardedByCurrentlyBeingPlayedCard = runtimePlayer.properties.Get<int>(PropertyKey.CARDS_DISCARDED_BY_CURRENTLY_BEING_PLAYED_CARD);
                cardsDiscardedByCurrentlyBeingPlayedCard.Value = cardController.HandPile.Cards.Count;
            }
       
            yield return cardController.DiscardRemainingCards();
        }
        
        public IEnumerator DiscardCard(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            // Update card state
            card.properties.Get<CardState>(PropertyKey.CARD_STATE).Value = CardState.DISCARD_PILE;
            
            // Update discard stats
            characterPlayingTheCard.properties.Get<int>(PropertyKey.CARDS_DISCARDED_ON_CURRENT_TURN_COUNT).Value++;
            characterPlayingTheCard.properties.Get<int>(PropertyKey.CARDS_DISCARDED_ON_CURRENT_BATTLE_COUNT).Value++;

            // Handle visuals
            yield return cardController.Discard(card.Card);

            // Handle game event (skills etc. can trigger here)
            yield return OnGameEvent(GameEvent.ON_CARD_DISCARDED, characterPlayingTheCard, player, enemies);
        }
        
        public IEnumerator DrawCard(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            // Handle visuals
            //Don't have enough card in deck, try get from discard pile
            if (cardController.DeckPile.Cards.Count < 1)
            {
                yield return ShuffleDiscardPileIntoDeck(player, enemies);
            }
            
            yield return cardController.Draw(1);

            // NOTE: Since cardController.Draw() doesn't return the drawn cards, I have to put this "hack" here.
            foreach (Card cardInHand in cardController.HandPile.Cards)
            {
                cardInHand.runtimeCard.properties.Get<CardState>(PropertyKey.CARD_STATE).Value = CardState.HAND;
            }
                        
            // Handle game event (skills etc. can trigger here)
            yield return OnGameEvent(GameEvent.ON_CARD_DRAWN, characterPlayingTheCard, player, enemies);
        }

        public IEnumerator CreateCardAndAddItToHand(RuntimeCard card)
        {
            yield return cardController.CreateCardAndAddItToHand(card);
        }
        
        public IEnumerator CreateCardAndAddItToDrawPile(RuntimeCard card)
        {
            yield return cardController.CreateCardAndAddItToDrawPile(card);
        }
        
        public IEnumerator CreateCardAndAddItToDiscardPile(RuntimeCard card)
        {
            yield return cardController.CreateCardAndAddItToDiscardPile(card);
        }

        public IEnumerator ShuffleDiscardPileIntoDeck(RuntimeCharacter player, List<RuntimeCharacter> enemies)
        {
            yield return cardController.ShuffleDiscardPileIntoDeck();

            yield return OnGameEvent(GameEvent.ON_CARD_SHUFFLED, player, player, enemies);
        }

        public IEnumerator ShuffleHandToDeck(RuntimeCard card, RuntimeCharacter player,
            List<RuntimeCharacter> enemies)
        {
            yield return cardController.ShuffleHandToDeck(card);
            
            yield return OnGameEvent(GameEvent.ON_CARD_SHUFFLED, player, player, enemies);
        }
        
        public IEnumerator DiscardCardAndDraw(RuntimeCard card, RuntimeCharacter player,
            List<RuntimeCharacter> enemies)
        {
            yield return DiscardCard(card, player, player, null, enemies);
            
            yield return DrawCard(null, player, player, null, enemies);
        }

        public IEnumerator WaitForSelectCardToShuffle(int count, RuntimeCharacter player,
        List<RuntimeCharacter> enemies)
        {
            while (count > 0 && cardController.HandPile.Cards.Count > 0)
            {
                //Prevent player playing card while selecting
                canPlayCard = false;
                if (Input.GetMouseButtonUp(0))
                {
                    var _hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero,
                        LayerMask.NameToLayer("Card"));

                    if (_hit)
                    {
                        var _target = _hit.transform.gameObject.GetComponent<Card>();
                        if (_target)
                        {
                            if (!_target.runtimeCard.IsPersist())
                            {
                                yield return DiscardCardAndDraw(_target.runtimeCard, player, enemies);
                                count--;
                            }
                            else
                            {
                                //TODO: play can't select feedback
                            }
                        }
                    }
                }

                yield return null;
            }

            canPlayCard = true;
        }

        /// <summary>
        /// Called when a character is killed (either player or enemy).
        /// </summary>
        public IEnumerator Kill(RuntimeCharacter characterToKill, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies, FXKey condition)
        {
            // TODO: VFX, animation etc. Remove the character from battle (if it's enemy)
            
            characterToKill.properties.Get<CharacterState>(PropertyKey.CHARACTER_STATE).Value = CharacterState.DEAD;
            
            if (characterToKill == player)
            {
                yield return characterToKill.Character.OnKilled(condition);
                runtimePlayer = null;
                //Game over
                yield return GameManager.Instance.GameOver();
            }
            else
            {
                runtimeEnemies.Remove(characterToKill);
                this.enemies.Remove(characterToKill.Character);
                
                yield return characterToKill.Character.OnKilled(condition);
            }

            if (runtimeEnemies.Count == 0)
            {
                //WIN!
            }
            
            yield return OnGameEvent(GameEvent.ON_DEATH, characterToKill, player, enemies);
        }

        public IEnumerator Decay(RuntimeCharacter target, RuntimeCharacter player, List<RuntimeCharacter> enemies)
        {
            // TODO: VFX
            target.Character.PlayParticle(FXKey.DECAY);
            
            Property<int> decay = target.properties.Get<int>(PropertyKey.DECAY);
            int decayAmount = decay.GetValueWithModifiers(target);
            
            FormData previousForm = target.GetCurrentForm();
            
            Property<int> stable = target.properties.Get<int>(PropertyKey.STABLE);
            Property<int> size = target.properties.Get<int>(PropertyKey.SIZE);
            Property<int> maxSize = target.properties.Get<int>(PropertyKey.MAX_SIZE);
            
            // Keep track of the previous size
            int previousSize = size.Value;
            
            // Calculate the the size change value after stable absorption (i.e. reduce stable value from size change value)
            int amountAbsorbedByStable = Mathf.Min(decayAmount, stable.Value);
            decayAmount -= amountAbsorbedByStable;
                
            // Reduce the absorbed size change value from the stable stack
            stable.Value = Mathf.Max(stable.Value - amountAbsorbedByStable, 0);

            // Reduce the target size by decay amount
            size.Value = Mathf.Clamp(size.Value - decayAmount, 0, maxSize.GetValueWithModifiers(target));

            // Reduce decay stack by 1
            decay.Value = Mathf.Clamp(decay.Value - 1, 0, int.MaxValue);
            
            FormData currentForm = target.GetCurrentForm();

            if (previousForm != currentForm)
            {
                yield return ChangeForm(previousForm, currentForm, target, player, enemies);
            }
            
            if (previousSize != size.Value)
            {
                yield return ChangeSize(previousSize, size.Value, target, player, enemies);
            }
        }
        
        public IEnumerator Grow(RuntimeCharacter target, RuntimeCharacter player, List<RuntimeCharacter> enemies)
        {
            // TODO: VFX
            target.Character.PlayParticle(FXKey.GROW);
            
            Property<int> grow = target.properties.Get<int>(PropertyKey.GROW);
            int growAmount = grow.GetValueWithModifiers(target);
            
            FormData previousForm = target.GetCurrentForm();
            
            Property<int> stable = target.properties.Get<int>(PropertyKey.STABLE);
            Property<int> size = target.properties.Get<int>(PropertyKey.SIZE);
            Property<int> maxSize = target.properties.Get<int>(PropertyKey.MAX_SIZE);
            
            // Keep track of the previous size
            int previousSize = size.Value;
            
            // Calculate the the size change value after stable absorption (i.e. reduce stable value from size change value)
            int amountAbsorbedByStable = Mathf.Min(growAmount, stable.Value);
            growAmount -= amountAbsorbedByStable;
                
            // Reduce the absorbed size change value from the stable stack
            stable.Value = Mathf.Max(stable.Value - amountAbsorbedByStable, 0);

            // Reduce the target size by grow amount
            size.Value = Mathf.Clamp(size.Value - growAmount, 0, maxSize.GetValueWithModifiers(target));

            // Reduce grow stack by 1
            grow.Value = Mathf.Clamp(grow.Value - 1, 0, int.MaxValue);
            
            FormData currentForm = target.GetCurrentForm();

            if (previousForm != currentForm)
            {
                yield return ChangeForm(previousForm, currentForm, target, player, enemies);
            }
            
            if (previousSize != size.Value)
            {
                yield return ChangeSize(previousSize, size.Value, target, player, enemies);
            }
        }
        
        public IEnumerator ChangeForm(FormData previousForm, FormData currentForm, RuntimeCharacter character, RuntimeCharacter player, List<RuntimeCharacter> enemies)
        {
            // TODO: VFX, animation etc
            character.DisablePassives(previousForm);
            character.EnablePassives(currentForm);
            
            character.properties.Get<int>(PropertyKey.FORM_CHANGED_COUNT_CURRENT_TURN).Value++;
            character.properties.Get<int>(PropertyKey.ENEMY_ATTACK_PATTERN_CARD_INDEX).Value = 0;
            
            character.Character.UpdateFormVisual(previousForm, currentForm);
            character.Character.UpdatePassiveIcon(previousForm, currentForm);

            yield return OnGameEvent(GameEvent.ON_FORM_CHANGED, character, player, enemies);
            
        }
        
        public IEnumerator ChangeSize(int previousSize, int currentSize, RuntimeCharacter character, RuntimeCharacter player, List<RuntimeCharacter> enemies)
        {
            // TODO: VFX, animation etc

            yield return character.Character.UpdateSize(previousSize, currentSize);
            
            // Unique event for when explicitly player's size changes
            if (character == player)
            {
                yield return OnGameEvent(GameEvent.ON_PLAYER_SIZE_CHANGED, character, player, enemies);
            }
            
            yield return OnGameEvent(GameEvent.ON_SIZE_CHANGED, character, player, enemies);
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
            enemy.properties.Get<int>(PropertyKey.EVASION).Value = 0;
            
            yield return enemy.Character.UpdateIntention(enemy.Character.GetIntention());
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
            FormData form = enemy.GetCurrentForm();

            // Get enemy's next intent (the next card they plan to use)
            Property<int> cardIndex = enemy.properties.Get<int>(PropertyKey.ENEMY_ATTACK_PATTERN_CARD_INDEX);
            
            // If enemy is stunned don't allow them to play any cards
            if (enemy.properties.Get<int>(PropertyKey.STUN).Value > 0)
            {
                enemy.Character.PlayParticle(FXKey.STUN);
                
                //Wait for particle to play for a while before skip
                yield return new WaitForSeconds(stunDuration);
            }
            else
            {
                if (cardIndex.Value > form.attackPattern.Count - 1)
                {
                    yield break;
                }
                CardData cardData = form.attackPattern[cardIndex.Value];

                // Create card instance from the card data
                RuntimeCard card = CardFactory.Create(cardData);

                foreach (EffectData effectData in card.cardData.effects)
                {
                    yield return effectData.Execute(card, enemy, player, player, enemies);
                }
            }

            // Increment the index by 1 (wrap back to 0 if needed)
            cardIndex.Value = (cardIndex.Value + 1) % form.attackPattern.Count;
        }
        
        /// <summary>
        /// Coroutine that should be called after enemy finished acting.
        /// </summary>
        /// <param name="enemy">The enemy whose turn just ended.</param>
        public IEnumerator EnemyTurnEnd(RuntimeCharacter enemy)
        {
            // If enemy has decay debuff
            if (enemy.properties.Get<int>(PropertyKey.DECAY).GetValueWithModifiers(enemy) > 0)
            {
                yield return Decay(enemy, runtimePlayer, runtimeEnemies);
            }
            
            // If enemy has grow buff
            if (enemy.properties.Get<int>(PropertyKey.GROW).GetValueWithModifiers(enemy) > 0)
            {
                yield return Grow(enemy, runtimePlayer, runtimeEnemies);
            }
            
            enemy.properties.Get<int>(PropertyKey.STUN).Value = 0;
            enemy.properties.Get<int>(PropertyKey.THORNS).Value = 0;

            // Clear properties that are only tracked per turn
            enemy.properties.Get<int>(PropertyKey.CARDS_DISCARDED_ON_CURRENT_TURN_COUNT).Value = 0;
            enemy.properties.Get<int>(PropertyKey.CARDS_DESTROYED_ON_CURRENT_TURN_COUNT).Value = 0;
            enemy.properties.Get<int>(PropertyKey.CARDS_FADED_ON_CURRENT_TURN_COUNT).Value = 0;
            
            yield return enemy.Character.UpdateIntention(enemy.Character.GetIntention());
            
            // Death by size (if player is at min size or max size at the end of the turn they will die)
            int minSize = runtimePlayer.properties.Get<int>(PropertyKey.MIN_SIZE).Value;
            int maxSize = runtimePlayer.properties.Get<int>(PropertyKey.MAX_SIZE).Value;
            int size = runtimePlayer.properties.Get<int>(PropertyKey.SIZE).Value;

            if (size == minSize)
            {
                yield return Kill(runtimePlayer, null, runtimePlayer, null, runtimeEnemies, FXKey.SMALL_DEATH);
            }
            else if (size == maxSize)
            {
                yield return Kill(runtimePlayer, null, runtimePlayer, null, runtimeEnemies, FXKey.BIG_DEATH);
            }
        }

        // TODO: Call this after current battle ended
        public void ClearCurrentBattleProperties(RuntimeCharacter player)
        {
            player.properties.Get<int>(PropertyKey.CARDS_DISCARDED_ON_CURRENT_BATTLE_COUNT).Value = 0;
            player.properties.Get<int>(PropertyKey.CARDS_DESTROYED_ON_CURRENT_BATTLE_COUNT).Value = 0;
            player.properties.Get<int>(PropertyKey.CARDS_FADED_ON_CURRENT_BATTLE_COUNT).Value = 0;
        }

        public IEnumerator OnGameEvent(GameEvent gameEvent, RuntimeCharacter character, RuntimeCharacter player, List<RuntimeCharacter> enemies)
        {
            yield return TryRechargeActiveSkills(gameEvent, character, player, enemies);
            yield return TryTriggerActiveSkills(gameEvent, character, player, enemies);
            
            //Update cards value when something happened
            cardController.UpdateCards();
            
            TryTriggerPassives(gameEvent, player, player, enemies);
            foreach (var _enemy in enemies)
            {
                TryTriggerPassives(gameEvent, _enemy, player, enemies);
                _enemy.Character.UpdateBuffsAndDebuffsVisual();
            }
            player.Character.UpdateBuffsAndDebuffsVisual();
        }

        public IEnumerator TryTriggerActiveSkills(GameEvent gameEvent, RuntimeCharacter character, RuntimeCharacter player, List<RuntimeCharacter> enemies)
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
                        //OnGameEvent already called 
                        yield return effectData.Execute(card, character, player, player, enemies);
                    }
                }
            }

            // Try triggering card active skills for the cards that are in player's hand
            for (int i = cardController.HandPile.Cards.Count - 1; i >= 0; i--)
            {
                Card card = cardController.HandPile.Cards[i];

                foreach (CardActiveEffect cardActiveSkill in card.runtimeCard.cardData.cardActiveSkills)
                {
                    if (TryTriggerCardActive(gameEvent, cardActiveSkill, player))
                    {
                        foreach (EffectData effectData in cardActiveSkill.onTriggerEffects)
                        {
                            yield return effectData.Execute(card.runtimeCard, character, player, player, enemies);
                        }
                    }
                }
            }
        }

        public bool TryTriggerCardActive(GameEvent gameEvent, CardActiveEffect cardActiveActiveEffect, RuntimeCharacter player)
        {
            // The current game event must match the skill's trigger game event.
            if (cardActiveActiveEffect.triggerGameEvent != gameEvent)
            {
                return false;
            }
            
            // If there are trigger conditions -> all must pass for the active skill to trigger!
            if (cardActiveActiveEffect.triggerConditions != null)
            {
                foreach (ConditionData condition in cardActiveActiveEffect.triggerConditions)
                {
                    // If any condition fails the active won't trigger.
                    if (!condition.Evaluate(gameEvent, player, player))
                    {
                        return false;
                    }
                }
            }

            // Either there are no conditions OR all conditions passed -> active skill triggers!
            return true;
        }

        public bool TriggerActiveSkill(GameEvent gameEvent, RuntimeSkill skill, RuntimeCharacter character, RuntimeCharacter player)
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
        
        public IEnumerator TryRechargeActiveSkills(GameEvent gameEvent, RuntimeCharacter character, RuntimeCharacter player, List<RuntimeCharacter> enemies)
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
        
        public bool RechargeActiveSkill(GameEvent gameEvent, RuntimeSkill skill, RuntimeCharacter character, RuntimeCharacter player)
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
        
        public void TryTriggerPassives(GameEvent gameEvent, RuntimeCharacter character, RuntimeCharacter player, List<RuntimeCharacter> enemies)
        {
            var form = character.GetCurrentForm();
            foreach (var passive in character.passiveSlots[form])
            {
                if (passive == null) continue;

                if (passive.passiveData.triggerGameEvent == gameEvent)
                {
                    if (TriggerPassive(gameEvent, passive, character, player))
                    {
                        character.EnablePassive(passive);
                    }
                    else
                    {
                        character.DisablePassive(passive);
                    }
                }
            }
        }

        public bool TriggerPassive(GameEvent gameEvent, RuntimePassive passive, RuntimeCharacter character, RuntimeCharacter player)
        {
            if (passive.passiveData.triggerGameEvent != gameEvent)
            {
                return false;
            }
            
            if (passive.passiveData.triggerConditions != null)
            {
                foreach (ConditionData condition in passive.passiveData.triggerConditions)
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
    }
}

