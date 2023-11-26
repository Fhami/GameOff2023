using System;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Conditions/Property Condition", fileName = "New Property Condition")]
    public class PropertyCondition : ConditionData
    {
        public PropertySource propertySource;
        public PropertyComparer propertyComparer;
        
        public override bool Evaluate(GameEvent gameEvent, RuntimeCharacter character, RuntimeCharacter player)
        {
            switch (propertySource)
            {
                case PropertySource.NONE:
                {
                    throw new NotSupportedException();
                }
                case PropertySource.SELF:
                {
                    return propertyComparer.Evaluate(character);
                    break;
                }
                case PropertySource.PLAYER:
                {
                    return propertyComparer.Evaluate(player);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return false;
        }
    }
}