using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Skill", fileName = "New Skill")]
    public class SkillData : ScriptableObject
    {
        [Expandable] public List<ConditionData> rechargeConditions;
        [Expandable] public List<ConditionData> triggerConditions;
        [Expandable] public CardData card;
    }
}