using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Buff/Evasion", fileName = "New Evasion")]
    public class Evasion : BuffData
    {
        public override string GetDescriptionWithModifier(RuntimeCharacter character)
        {
            int evasion = character.properties.Get<int>(PropertyKey.EVASION).GetValueWithModifiers(character);
            return $"Prevent the next {evasion} times this creature would lose HP. Lose 1 stack when triggered.";
        }

        public override string GetDescription(int value)
        {
            return $"Prevent the next {value} times this creature would lose HP. Lose 1 stack when triggered.";
        }
    }
}