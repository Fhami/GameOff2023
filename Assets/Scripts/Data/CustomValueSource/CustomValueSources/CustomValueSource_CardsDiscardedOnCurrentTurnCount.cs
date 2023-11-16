using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Custom Value Source/Cards Discarded On Current Turn Count", fileName = "Cards Discarded On Current Turn Count")]
    public class CustomValueSource_CardsDiscardedOnCurrentTurnCount : CustomValueSource
    {
        public override int GetValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            return characterPlayingTheCard.properties.Get<int>(PropertyKey.CARDS_DISCARDED_ON_CURRENT_TURN_COUNT).Value;
        }
    }
}