using System;

namespace DefaultNamespace
{
    public static class PropertyExtensions
    {
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