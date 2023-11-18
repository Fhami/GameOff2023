using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Buff/Unstable", fileName = "New Unstable")]
    public class Unstable : BuffData
    {
        public override string GetDescription(RuntimeCharacter character)
        {
            int unstable = character.properties.Get<int>(PropertyKey.UNSTABLE).GetValueWithModifiers(character);
            return $"At the end of the turn, randomly increase or decrease your Size between -{unstable} to +{unstable}.";
        }
    }
}