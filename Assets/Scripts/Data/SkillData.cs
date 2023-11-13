using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Skill", fileName = "New Skill")]
    public class SkillData : ScriptableObject
    {
        [Header("Triggering")]
        public GameEvent triggerGameEvent;
        [Expandable] public List<ConditionData> triggerConditions;
        [Header("Recharging")]
        public GameEvent rechargeGameEvent;
        [Expandable] public List<ConditionData> rechargeConditions;
        [Header("The card (and it's effects) that will be played")]
        [Expandable] public CardData card;
    }
}