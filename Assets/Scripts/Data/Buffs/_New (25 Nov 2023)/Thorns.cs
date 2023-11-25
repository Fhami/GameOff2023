using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Buff/Thorns", fileName = "New Thorns")]
    public class Thorns : BuffData
    {
        public override string GetDescription(RuntimeCharacter character)
        {
            int thorns = character.properties.Get<int>(PropertyKey.THORNS).GetValueWithModifiers(character);
            return $"When attacked, deal {thorns} damage back to its attacker.";
        }
    }
}