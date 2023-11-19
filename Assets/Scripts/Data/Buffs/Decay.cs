using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Buff/Decay", fileName = "New Decay")]
    public class Decay : BuffData
    {
        public override string GetDescription(RuntimeCharacter character)
        {
            int decay = character.properties.Get<int>(PropertyKey.DECAY).GetValueWithModifiers(character);
            return $"At the end of the turn, Size -{decay.ToString()}.";
        }
    }
}