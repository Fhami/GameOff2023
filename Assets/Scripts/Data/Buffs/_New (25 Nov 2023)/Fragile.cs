using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Buff/Fragile", fileName = "New Fragile")]
    public class Fragile : BuffData
    {
        public override string GetDescription(RuntimeCharacter character)
        {
            int fragile = character.properties.Get<int>(PropertyKey.FRAGILE).GetValueWithModifiers(character);
            return $"Shield gained from cards reduced by 25%. At the end of its turn, lose 1 stack.";
        }
    }
}