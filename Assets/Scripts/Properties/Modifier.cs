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
        
        public bool IsConditional()
        {
            return conditions is { Count: > 0 };
        }

        // TODO: I think we need to add more info about the game state here so the modifier conditions can have more information about the battle
        public bool Evaluate(RuntimeEntity propertyOwner)
        {
            foreach (Condition condition in conditions)
            {
                if (!condition.Evaluate(propertyOwner))
                {
                    return false;
                }
            }
            
            return true;
        }
    }

    [Serializable]
    public class Condition
    {
        // TODO: This class needs a lot of work. Maybe the condition needs to be a ScriptableObject
        public PropertyKey propertyKey;
        public Comparison comparison;
        public int value;

        public bool Evaluate(RuntimeEntity propertyOwner)
        {
            // TODO: Handle condition evaluation
            throw new NotImplementedException();
        }
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