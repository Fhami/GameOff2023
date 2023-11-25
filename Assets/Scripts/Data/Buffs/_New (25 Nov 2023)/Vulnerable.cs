using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Buff/Vulnerable", fileName = "New Vulnerable")]
    public class Vulnerable : BuffData
    {
        public override string GetDescription(RuntimeCharacter character)
        {
            int vulnerable = character.properties.Get<int>(PropertyKey.VULNERABLE).GetValueWithModifiers(character);
            return $"Target takes 50% more damage from attacks. At the end of its turn, remove 1 stack.";
        }
    }
}