using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Buff/Fragile", fileName = "New Fragile")]
    public class Fragile : BuffData
    {
        public override string GetDescriptionWithModifier(RuntimeCharacter character)
        {
            int fragile = character.properties.Get<int>(PropertyKey.FRAGILE).GetValueWithModifiers(character);
            return $"<color={Colors.COLOR_STATUS}>Shield</color> gained from cards reduced by 25%. At the end of its turn, lose 1 stack.";
        }

        public override string GetDescription(int value)
        {
            return $"<color={Colors.COLOR_STATUS}>Shield</color> gained from cards reduced by 25%. At the end of its turn, lose 1 stack.";
        }
    }
}