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
        public List<PropertyComparer> conditions;
        
        public bool IsConditional()
        {
            return conditions is { Count: > 0 };
        }

        // TODO: I think we need to add more info about the game state here so the modifier conditions can have more information about the battle
        public bool Evaluate(RuntimeEntity propertyOwner)
        {
            foreach (PropertyComparer condition in conditions)
            {
                if (!condition.Evaluate(propertyOwner))
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}