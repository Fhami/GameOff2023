using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Buff/Envenomed", fileName = "New Envenomed")]
    public class Envenomed : BuffData
    {
        public override string GetDescription(RuntimeCharacter character)
        {
            int envenomed = character.properties.Get<int>(PropertyKey.ENVENOMED).GetValueWithModifiers(character);
            return $"Whenever this creature deals damage to your HP, add {envenomed} Poison card into your discard pile. At the end of the turn, remove 1 stack.";
        }
    }
}