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