using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Buff/Weak", fileName = "New Weak")]
    public class Weak : BuffData
    {
        public override string GetDescriptionWithModifier(RuntimeCharacter character)
        {
            int weak = character.properties.Get<int>(PropertyKey.WEAK).GetValueWithModifiers(character);
            return $"Deals 25% less attack damage. At the end of its turn, remove 1 stack.";
        }

        public override string GetDescription(int value)
        {
            return $"Deals 25% less attack damage. At the end of its turn, remove 1 stack.";
        }
    }
}