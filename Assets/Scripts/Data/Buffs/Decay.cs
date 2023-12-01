using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Buff/Decay", fileName = "New Decay")]
    public class Decay : BuffData
    {
        public override string GetDescriptionWithModifier(RuntimeCharacter character)
        {
            int decay = character.properties.Get<int>(PropertyKey.DECAY).GetValueWithModifiers(character);
            return $"At the end of the turn, <color={Colors.COLOR_STATUS}>Size</color> -{decay.ToString()}.";
        }

        public override string GetDescription(int value)
        {
            return $"At the end of the turn, <color={Colors.COLOR_STATUS}>Size</color> -{value.ToString()}.";
        }
    }
}