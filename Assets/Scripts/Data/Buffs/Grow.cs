using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Buff/Grow", fileName = "New Grow")]
    public class Grow : BuffData
    {
        public override string GetDescription(RuntimeCharacter character)
        {
            int grow = character.properties.Get<int>(PropertyKey.GROW).GetValueWithModifiers(character);
            return $"At the end of the turn, Size +{grow.ToString()}.";
        }
    }
}