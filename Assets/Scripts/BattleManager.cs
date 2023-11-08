using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    // TODO: Maybe should rename this to GameEvent and include other events here too? So they can be used for conditions
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
        private void Awake()
        {
            Debug.Log("Hello from BattleManager!");
        }

        // TODO: This should be called when player turn starts
        public IEnumerator PlayerTurnStart(RuntimeCharacter player)
        {
            player.TryGetCurrentForm(out FormData form);

            // Clear player shield stack
            // TODO: If we have artifacts like "Don't remove shield at turn start" you can do it here.
            player.properties.Get<int>(PropertyKey.SHIELD).Value = 0;
            
            // Reset player's action points to the base action point value of their current form
            player.properties.Get<int>(PropertyKey.ACTION_POINTS).Value = form.actionPoints;
            // NOTE: If we want we could have similar property like POWER_UP but for action points (if some skill gives extra AP or reduces AP) then just calculate it here
            
            // TODO: Execute PLAYER_TURN_START skills/effects
            
            // TODO: Draw cards based on player action point value?
            
            yield break;
        }

        // TODO: This should be called when player turn ends
        public IEnumerator PlayerTurnEnd(RuntimeCharacter player)
        {
            player.TryGetCurrentForm(out FormData form);
            
            // TODO: Execute PLAYER_TURN_END skills/effects
            
            Property<int> power = player.properties.Get<int>(PropertyKey.POWER);
            Property<int> maxPower = player.properties.Get<int>(PropertyKey.MAX_POWER);
            
            // TODO: Handle overload logic and overload effects. QUESTION: Do we allow power go over max power or do we cap it at max power?
            if (power.Value > maxPower.Value)
            {
                throw new NotImplementedException();
            }
            if (power.Value <= 0)
            {
                // TODO: Handle player death when they turn into dust
                throw new NotImplementedException();
            }
            
            // Clear player properties that are only tracked per turn
            player.properties.Get<int>(PropertyKey.CARDS_DISCARDED_ON_CURRENT_TURN_COUNT).Value = 0;
            player.properties.Get<int>(PropertyKey.CARDS_DESTROYED_ON_CURRENT_TURN_COUNT).Value = 0;
            player.properties.Get<int>(PropertyKey.CARDS_FADED_ON_CURRENT_TURN_COUNT).Value = 0;
            
            yield break;
        }
        
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
        
        // TODO: Call this after current battle ended
        public void ClearCurrentBattleProperties(RuntimeCharacter player)
        {
            player.properties.Get<int>(PropertyKey.CARDS_DISCARDED_ON_CURRENT_BATTLE_COUNT).Value = 0;
            player.properties.Get<int>(PropertyKey.CARDS_DESTROYED_ON_CURRENT_BATTLE_COUNT).Value = 0;
            player.properties.Get<int>(PropertyKey.CARDS_FADED_ON_CURRENT_BATTLE_COUNT).Value = 0;
        }
    }
}

