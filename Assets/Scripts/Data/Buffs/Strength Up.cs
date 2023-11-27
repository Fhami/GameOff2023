using UnityEngine;

namespace DefaultNamespace.Buffs
{
    [CreateAssetMenu(menuName = "Gamejam/Buff/Strength", fileName = "New Strength")]
    public class Strength : BuffData
    {
        public override string GetDescriptionWithModifier(RuntimeCharacter character)
        {
            int strength = character.properties.Get<int>(PropertyKey.STRENGTH).GetValueWithModifiers(character);
            return $"Increase attack damage by {strength.ToString()}.";
        }

        public override string GetDescription(int value)
        {
            return $"Increase attack damage by {value.ToString()}.";
        }
    }
}