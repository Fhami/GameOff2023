using System;

namespace DefaultNamespace
{
    public static class Colors
    {
        public static string COLOR_STATUS = "yellow";
        public static string COLOR_NUMBER_NORMAL = "white";
        public static string COLOR_NUMBER_INCREASED = "orange";
        public static string COLOR_NUMBER_DECREASED = "#56A6FF";

        public static string GetNumberColor(ValueState valueState)
        {
            switch (valueState)
            {
                case ValueState.NORMAL:
                    return COLOR_NUMBER_NORMAL;
                case ValueState.INCREASED:
                    return COLOR_NUMBER_INCREASED;
                case ValueState.DECREASED:
                    return COLOR_NUMBER_DECREASED;
                default:
                    throw new ArgumentOutOfRangeException(nameof(valueState), valueState, null);
            }
        }
    }
}