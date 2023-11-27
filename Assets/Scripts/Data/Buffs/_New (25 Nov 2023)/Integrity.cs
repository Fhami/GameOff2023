using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Buff/Integrity", fileName = "New Integrity")]
    public class Integrity : BuffData
    {
        public override string GetDescriptionWithModifier(RuntimeCharacter character)
        {
            int integrity = character.properties.Get<int>(PropertyKey.INTEGRITY).GetValueWithModifiers(character);
            return $"Shield is not removed at the start of turn.";
        }

        public override string GetDescription(int value)
        {
            return $"Shield is not removed at the start of turn.";
        }
    }
}