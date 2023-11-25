using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Buff/Evasion", fileName = "New Evasion")]
    public class Evasion : BuffData
    {
        public override string GetDescription(RuntimeCharacter character)
        {
            int evasion = character.properties.Get<int>(PropertyKey.EVASION).GetValueWithModifiers(character);
            return $"Prevent the next {evasion} times this creature would lose HP. Lose 1 stack when triggered.";
        }
    }
}