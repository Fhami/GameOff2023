using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Buff/Regen", fileName = "New Regen")]
    public class Regen : BuffData
    {
        public override string GetDescription(RuntimeCharacter character)
        {
            int regen = character.properties.Get<int>(PropertyKey.REGEN).GetValueWithModifiers(character);
            return $"At the end of its turn, heal {regen}, then remove 1 stack.";
        }
    }
}