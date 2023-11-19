using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Card Skill", fileName = "New Card Skill")]
    public class CardSkill : ScriptableObject
    {
        [Header("Triggering")]
        public GameEvent triggerGameEvent;
        [Expandable] public List<ConditionData> triggerConditions;
        public List<EffectData> onTriggerEffects;
    }
}