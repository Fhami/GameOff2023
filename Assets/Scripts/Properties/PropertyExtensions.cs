using System;

namespace DefaultNamespace
{
    public static class PropertyExtensions
    {
        public static int GetValueWithModifiers(this Property<int> property, RuntimeEntity context)
        {
            // If there are no modifiers attached to this property, just return the original value
            if (property.Modifiers == null)
            {
                return property.Value;
            }

            int value = property.Value;

            foreach (Modifier modifier in property.Modifiers)
            {
                // If the modifier has conditions -> only apply the modifier if ALL of the conditions pass
                if (modifier.IsConditional() && !modifier.Evaluate(context))
                {
                    continue;
                }

                // Execute the operation
                switch (modifier.operation)
                {
                    case Operation.INCREASE:
                        value += modifier.value;
                        break;
                    case Operation.DECREASE:
                        value -= modifier.value;
                        break;
                    case Operation.SET:
                        value = modifier.value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return value;
        }
        
        public static void ApplyOperation(this Property<int> intProperty, Operation operation, int value)
        {
            switch (operation)
            {
                case Operation.INCREASE:
                    intProperty.Value += value;
                    break;
                case Operation.DECREASE:
                    intProperty.Value -= value;
                    break;
                case Operation.SET:
                    intProperty.Value = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
            }
        }
    }
}