using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Buff/StrengthDown", fileName = "New StrengthDown")]
    public class StrengthDown : BuffData
    {
        public override string GetDescription(RuntimeCharacter character)
        {
            int strength_down = character.properties.Get<int>(PropertyKey.STRENGTH_DOWN).GetValueWithModifiers(character);
            return $"Decrease attack damage by {strength_down}.";
        }
    }
}