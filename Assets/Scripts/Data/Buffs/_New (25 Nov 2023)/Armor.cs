using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Buff/Armor", fileName = "New Armor")]
    public class Armor : BuffData
    {
        public override string GetDescriptionWithModifier(RuntimeCharacter character)
        {
            int armor = character.properties.Get<int>(PropertyKey.ARMOR).GetValueWithModifiers(character);
            return $"At the end of your turn, gain {armor} <color={Colors.COLOR_STATUS}>Shield</color>. Receiving unblocked attack damage reduces Plated Armor by 1.";
        }

        public override string GetDescription(int value)
        {
            return $"At the end of your turn, gain {value} <color={Colors.COLOR_STATUS}>Shield</color>. Receiving unblocked attack damage reduces Plated Armor by 1.";
        }
    }
}