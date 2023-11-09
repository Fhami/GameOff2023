using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Form", fileName = "New Form")]
    public class FormData : ScriptableObject
    {
        [InfoBox("powerMin (inclusive) and powerMax (inclusive) defines the power range when this form is active.")]
        public int powerMin;
        public int powerMax;
        public int actionPoints;
        [Expandable] public List<SkillData> skills;
        [InfoBox("The cards the enemy character uses in this form.")]
        [Expandable] public List<CardData> attackPattern;
    }
}