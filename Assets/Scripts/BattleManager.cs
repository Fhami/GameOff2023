using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public enum CardEvent
    {
        NONE,
        CARD_DRAWN,
        CARD_DISCARDED,
        CARD_DESTROYED,
        CARD_FADED,
        CARD_PLAYED
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

        public IEnumerator PlayCard(RuntimeCard card, RuntimeCharacter player, RuntimeCharacter target, List<RuntimeCharacter> enemies)
        {
            card.cardState = CardState.PLAYING;
            
            // TODO: Execute passive abilities that trigger on CardEvent.CARD_PLAYED (I guess we do this here before executing this card's effects?)

            // Execute card effects one by one
            foreach (EffectData effectData in card.cardData.effects)
            {
                yield return effectData.Execute(card, player, target, enemies);

                // Exit early if the card was FADED or DESTROYED (so we don't try to execute effects on invalid card)
                if (card.cardState is CardState.FADED or CardState.DESTROYED)
                {
                    yield break;
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

        // TODO: Call this after current turn ended
        public void ClearCurrentTurnProperties(RuntimeCharacter player)
        {
            player.properties.Get<int>(PropertyKey.CARDS_DISCARDED_ON_CURRENT_TURN_COUNT).Value = 0;
            player.properties.Get<int>(PropertyKey.CARDS_DESTROYED_ON_CURRENT_TURN_COUNT).Value = 0;
            player.properties.Get<int>(PropertyKey.CARDS_FADED_ON_CURRENT_TURN_COUNT).Value = 0;
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

