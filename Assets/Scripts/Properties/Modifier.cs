using System;
using System.Collections.Generic;

namespace DefaultNamespace
{
    [Serializable]
    public class Modifier
    {
        public PropertyKey propertyKey;
        public Operation operation;
        public int value;
        public List<Condition> conditions;
    }

    [Serializable]
    public class Condition
    {
        public PropertyKey propertyKey;
        public Comparison comparison;
        public int value;
    }

    public enum Comparison
    {
        EQUAL_TO,
        NOT_EQUAL_TO,
        GREATER_THAN,
        LESS_THAN,
        GREATER_THAN_OR_EQUAL_TO,
        LESS_THAN_OR_EQUAL_TO
    }
}