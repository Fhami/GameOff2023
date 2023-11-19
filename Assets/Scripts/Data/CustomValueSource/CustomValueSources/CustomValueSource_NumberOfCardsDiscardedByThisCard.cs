using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Custom Value Source/Number Of Cards Discarded By This Card", fileName = "Number Of Cards Discarded By This Card")]
    public class CustomValueSource_NumberOfCardsDiscardedByThisCard : CustomValueSource
    {
        public override int GetValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            Property<CardState> cardState = card.properties.Get<CardState>(PropertyKey.CARD_STATE);

            // If the card is still in your hand, display the number of cards in your hand that would be discarded.
            if (cardState.Value == CardState.HAND)
            {
                return BattleManager.current.CardController.HandPile.Cards.Count;
            }
            
            // Otherwise display the amount of cards discarded by the card currently being played.
            return characterPlayingTheCard.properties.Get<int>(PropertyKey.CARDS_DISCARDED_BY_CURRENTLY_BEING_PLAYED_CARD).Value;
        }
    }
}