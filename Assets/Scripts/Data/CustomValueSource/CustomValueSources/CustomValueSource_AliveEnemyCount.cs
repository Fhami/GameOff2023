using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Custom Value Source/Alive Enemy Count", fileName = "Alive Enemy Count")]
    public class CustomValueSource_AliveEnemyCount : CustomValueSource
    {
        public override int GetValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            int value = 0;
            
            if (BattleManager.current != null)
            {
                foreach (Character enemy in BattleManager.current.enemies)
                {
                    Property<CharacterState> characterState = enemy.runtimeCharacter.properties.Get<CharacterState>(PropertyKey.CHARACTER_STATE);
                    if (characterState.Value == CharacterState.ALIVE)
                    {
                        value++;
                    }
                }
            }

            return value;
        }
    }
}