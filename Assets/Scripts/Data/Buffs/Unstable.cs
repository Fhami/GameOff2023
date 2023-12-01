using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Buff/Unstable", fileName = "New Unstable")]
    public class Unstable : BuffData
    {
        public override string GetDescriptionWithModifier(RuntimeCharacter character)
        {
            int unstable = character.properties.Get<int>(PropertyKey.UNSTABLE).GetValueWithModifiers(character);
            return $"At the end of the turn, randomly increase or decrease your <color={Colors.COLOR_STATUS}>Size</color> between -{unstable} to +{unstable}.";
        }

        public override string GetDescription(int value)
        {
            return $"At the end of the turn, randomly increase or decrease your <color={Colors.COLOR_STATUS}>Size</color> between -{value} to +{value}.";
        }
    }
}