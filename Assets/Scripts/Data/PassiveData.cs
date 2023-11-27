using System.Collections.Generic;
using System.Text;
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

        public string GetDescription()
        {
            var sb = new StringBuilder();
            if (triggerGameEvent != GameEvent.NONE)
            {
                sb.Append("When ");
                foreach (var _condition in triggerConditions)
                {
                    sb.Append(_condition.name);
                    sb.Append(", ");
                }
            }

            sb.Append(buffData.GetDescription(value));

            return sb.ToString();
        }
    }
}