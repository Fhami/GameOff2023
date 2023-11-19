using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Custom Value Source/Number Of Cards Discarded By This Card", fileName = "Number Of Cards Discarded By This Card")]
    public class CustomValueSource_NumberOfCardsDiscardedByThisCard : CustomValueSource
    {
        public override int GetValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            return characterPlayingTheCard.properties.Get<int>(PropertyKey.CARDS_DISCARDED_BY_CURRENTLY_BEING_PLAYED_CARD).Value;
        }
    }
}