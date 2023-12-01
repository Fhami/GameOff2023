using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Buff/Grow", fileName = "New Grow")]
    public class Grow : BuffData
    {
        public override string GetDescriptionWithModifier(RuntimeCharacter character)
        {
            int grow = character.properties.Get<int>(PropertyKey.GROW).GetValueWithModifiers(character);
            return $"At the end of the turn, <color={Colors.COLOR_STATUS}>Size</color> +{grow.ToString()}.";
        }

        public override string GetDescription(int value)
        {
            return $"At the end of the turn, <color={Colors.COLOR_STATUS}>Size</color> +{value.ToString()}.";
        }
    }
}