using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace DefaultNamespace
{
    public enum Size
    {
        S,
        M,
        L
    }
    
    [CreateAssetMenu(menuName = "Gamejam/Form", fileName = "New Form")]
    public class FormData : ScriptableObject
    {
        [InfoBox("sizeMin (inclusive) and sizeMax (inclusive) defines the size range when this form is active.")]
        public int sizeMin;
        public int sizeMax;
        public Size size;
        [InfoBox("Player's active skill that will available when size match value")]
        public CardData activeSkill;
        [Header("The passives the enemy has active in this form. Note that player's passives are NOT defined here.")]
        [Expandable] public List<PassiveData> passives;
        [Expandable] public List<SkillData> skills;
        [InfoBox("The cards the enemy character uses in this form.")]
        [Expandable] public List<CardData> attackPattern;
    }
}