using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Buff/ShieldUp", fileName = "New ShieldUp")]
    public class ShieldUp : BuffData
    {
        public override string GetDescription(RuntimeCharacter character)
        {
            int shield_up = character.properties.Get<int>(PropertyKey.SHIELD_UP).GetValueWithModifiers(character);
            return $"Increase Shield gained from cards by {shield_up}.";
        }
    }
}