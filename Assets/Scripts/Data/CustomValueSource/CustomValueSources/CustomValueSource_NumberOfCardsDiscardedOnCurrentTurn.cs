using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Custom Value Source/Number Of Cards Discarded On Current Turn", fileName = "Number Of Cards Discarded On Current Turn")]
    public class CustomValueSource_NumberOfCardsDiscardedOnCurrentTurn : CustomValueSource
    {
        public override int GetValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            return characterPlayingTheCard.properties.Get<int>(PropertyKey.CARDS_DISCARDED_ON_CURRENT_TURN_COUNT).Value;
        }
    }
}