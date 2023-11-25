using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Buff/Armor", fileName = "New Armor")]
    public class Armor : BuffData
    {
        public override string GetDescription(RuntimeCharacter character)
        {
            int armor = character.properties.Get<int>(PropertyKey.ARMOR).GetValueWithModifiers(character);
            return $"At the end of your turn, gain {armor} Shield. Receiving unblocked attack damage reduces Plated Armor by 1.";
        }
    }
}