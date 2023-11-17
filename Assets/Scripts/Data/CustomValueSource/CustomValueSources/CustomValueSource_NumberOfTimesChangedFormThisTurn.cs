using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Custom Value Source/Number Of Times Changed Form This Turn", fileName = "Number Of Times Changed Form This Turn")]
    public class CustomValueSource_NumberOfTimesChangedFormThisTurn : CustomValueSource
    {
        public override int GetValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            return characterPlayingTheCard.properties.Get<int>(PropertyKey.FORM_CHANGED_COUNT_CURRENT_TURN).Value;
        }
    }
}