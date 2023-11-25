using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Buff/ShieldDown", fileName = "New ShieldDown")]
    public class shield_down : BuffData
    {
        public override string GetDescription(RuntimeCharacter character)
        {
            int shield_down = character.properties.Get<int>(PropertyKey.SHIELD_DOWN).GetValueWithModifiers(character);
            return $"Decrease Shield gained from cards by {shield_down}.";
        }
    }
}