using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Buff/Stable", fileName = "New Stable")]
    public class Stable : BuffData
    {
        public override string GetDescription(RuntimeCharacter character)
        {
            int stable = character.properties.Get<int>(PropertyKey.STABLE).GetValueWithModifiers(character);
            return $"Prevents {stable} Size change. Remove ALL stacks at the end of its turn.";
        }
    }
}