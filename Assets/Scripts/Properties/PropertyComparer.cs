using System;

namespace DefaultNamespace
{
    [Serializable]
    public class PropertyComparer
    {
        public PropertyKey propertyKey;
        public Comparison comparison;
        public int value;

        public bool Evaluate(RuntimeEntity propertyOwner)
        {
            Property<int> property = propertyOwner.properties.Get<int>(propertyKey);
            
            switch (comparison)
            {
                case Comparison.EQUAL_TO:
                    return property.GetValueWithModifiers(propertyOwner) == value;
                case Comparison.NOT_EQUAL_TO:
                    return property.GetValueWithModifiers(propertyOwner) != value;
                case Comparison.GREATER_THAN:
                    return property.GetValueWithModifiers(propertyOwner) > value;
                case Comparison.LESS_THAN:
                    return property.GetValueWithModifiers(propertyOwner) < value;
                case Comparison.GREATER_THAN_OR_EQUAL_TO:
                    return property.GetValueWithModifiers(propertyOwner) >= value;
                case Comparison.LESS_THAN_OR_EQUAL_TO:
                    return property.GetValueWithModifiers(propertyOwner) <= value;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}