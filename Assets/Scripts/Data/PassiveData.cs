using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Passive", fileName = "New Passive")]
    public class PassiveData : ScriptableObject
    {
        [Header("Triggering")]
        public GameEvent triggerGameEvent;
        public List<ConditionData> triggerConditions;
        
        public BuffData buffData;
        public PropertyKey propertyKey;
        public Operation operation;
        public int value;
    }
}