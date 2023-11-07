using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Form", fileName = "New Form")]
    public class FormData : ScriptableObject
    {
        public int sizeMin;
        public int sizeMax;
        public int actionPoints;
        [Expandable] public List<SkillData> skills;
    }
}