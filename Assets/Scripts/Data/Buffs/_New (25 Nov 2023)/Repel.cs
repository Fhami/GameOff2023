using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Buff/Repel", fileName = "New Repel")]
    public class Repel : BuffData
    {
        public override string GetDescriptionWithModifier(RuntimeCharacter character)
        {
            int repel = character.properties.Get<int>(PropertyKey.REPEL).GetValueWithModifiers(character);
            return $"Negate {repel} debuff. Remove 1 stack when triggered.";
        }

        public override string GetDescription(int value)
        {
            return $"Negate {value} debuff. Remove 1 stack when triggered.";
        }
    }
}