using System;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Conditions/Property Condition", fileName = "New Property Condition")]
    public class PropertyCondition : ConditionData
    {
        public PropertySource propertySource;
        public PropertyComparer propertyComparer;
        
        public override bool Evaluate(GameEvent gameEvent, RuntimeCharacter player, RuntimeCharacter target)
        {
            switch (propertySource)
            {
                case PropertySource.NONE:
                {
                    throw new NotSupportedException();
                }
                case PropertySource.PLAYER:
                {
                    propertyComparer.Evaluate(player);
                    break;
                }
                case PropertySource.TARGET:
                {
                    propertyComparer.Evaluate(target);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return false;
        }
    }
}