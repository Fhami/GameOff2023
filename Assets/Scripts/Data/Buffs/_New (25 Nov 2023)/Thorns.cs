using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Buff/Thorns", fileName = "New Thorns")]
    public class Thorns : BuffData
    {
        public override string GetDescriptionWithModifier(RuntimeCharacter character)
        {
            int thorns = character.properties.Get<int>(PropertyKey.THORNS).GetValueWithModifiers(character);
            return $"When attacked, deal {thorns} damage back to its attacker.";
        }

        public override string GetDescription(int value)
        {
            return $"When attacked, deal {value} damage back to its attacker.";
        }
    }
}