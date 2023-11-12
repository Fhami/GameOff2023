using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Form", fileName = "New Form")]
    public class FormData : ScriptableObject
    {
        [InfoBox("sizeMin (inclusive) and sizeMax (inclusive) defines the size range when this form is active.")]
        public int sizeMin;
        public int sizeMax;
        public int actionPoints;
        [Expandable] public List<SkillData> skills;
        [InfoBox("The cards the enemy character uses in this form.")]
        [Expandable] public List<CardData> attackPattern;
    }
}