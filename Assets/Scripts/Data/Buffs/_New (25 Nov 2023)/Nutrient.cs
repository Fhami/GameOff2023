using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Buff/Nutrient", fileName = "New Nutrient")]
    public class Nutrient : BuffData
    {
        public override string GetDescription(RuntimeCharacter character)
        {
            int nutrient = character.properties.Get<int>(PropertyKey.NUTRIENT).GetValueWithModifiers(character);
            return $"At the end of its turn, gain {nutrient} Strength.";
        }
    }
}