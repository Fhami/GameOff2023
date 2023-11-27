using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Buff/Protection", fileName = "New Protection")]
    public class Protection : BuffData
    {
        public override string GetDescriptionWithModifier(RuntimeCharacter character)
        {
            int protection = character.properties.Get<int>(PropertyKey.PROTECTION).GetValueWithModifiers(character);
            return $"At the end of its turn, gain {protection} Shield.";
        }

        public override string GetDescription(int value)
        {
            return $"At the end of its turn, gain {value} Shield.";
        }
    }
}