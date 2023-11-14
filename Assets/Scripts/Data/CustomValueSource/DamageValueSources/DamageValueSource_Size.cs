using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Value Source/Damage Value Source/Size", fileName = "DamageValueSource_Size")]
    public class DamageValueSource_Size : DamageValueSource
    {
        public override int GetValue(RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player,
            RuntimeCharacter cardTarget,
            List<RuntimeCharacter> enemies)
        {
            return characterPlayingTheCard.properties.Get<int>(PropertyKey.SIZE).Value;
        }

        public override string GetDescription()
        {
            return "Deal damage equal to your size";
        }
    }
}