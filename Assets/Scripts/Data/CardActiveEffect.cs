using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Card Active Effect", fileName = "New Card Active Effect")]
    public class CardActiveEffect : ScriptableObject
    {
        [Header("Triggering")]
        public GameEvent triggerGameEvent;
        [Expandable] public List<ConditionData> triggerConditions;
        public List<EffectData> onTriggerEffects;
    }
}