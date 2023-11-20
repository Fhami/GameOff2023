using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Custom Value Source/Player Size", fileName = "Player Size")]
    public class CustomValueSource_PlayerSize : CustomValueSource
    {
        public override int GetValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            return player.properties.Get<int>(PropertyKey.SIZE).Value;
        }
    }
}