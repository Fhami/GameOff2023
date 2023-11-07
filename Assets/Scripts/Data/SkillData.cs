using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Skill", fileName = "New Skill")]
    public class SkillData : ScriptableObject
    {
        public int size;
        public List<ConditionData> conditions;
    }
}