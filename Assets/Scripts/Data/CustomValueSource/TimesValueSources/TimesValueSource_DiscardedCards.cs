using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Value Source/Times Value Source/Discarded Cards", fileName = "TimesValueSource_DiscardedCards")]
    public class TimesValueSource_DiscardedCards : TimesValueSource
    {
        public override int GetValue(RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player,
            RuntimeCharacter cardTarget,
            List<RuntimeCharacter> enemies)
        {
            return characterPlayingTheCard.properties.Get<int>(PropertyKey.CARDS_DISCARDED_ON_CURRENT_TURN_COUNT).Value;
        }

        public override string GetDescription()
        {
            return " number of times you've discarded cards this turn.";
        }
    }
}