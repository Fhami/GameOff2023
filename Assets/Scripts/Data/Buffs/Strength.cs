using UnityEngine;

namespace DefaultNamespace.Buffs
{
    [CreateAssetMenu(menuName = "Gamejam/Buff/Strength", fileName = "New Strength")]
    public class Strength : BuffData
    {
        public override string GetDescription(RuntimeCharacter character)
        {
            int strength = character.properties.Get<int>(PropertyKey.STRENGTH).GetValueWithModifiers(character);
            return $"Increase attack damage by {strength.ToString()}.";
        }
    }
}