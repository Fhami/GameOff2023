using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Gain Shield Effect", fileName = "New Gain Shield Effect")]
    public class GainShieldEffect : EffectData
    {
        public int value;
        
        public override IEnumerator Execute(
            RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player,
            RuntimeCharacter cardTarget,
            List<RuntimeCharacter> enemies)
        {
            // TODO: VFX

            characterPlayingTheCard.properties.Get<int>(PropertyKey.SHIELD).Value += value;
            
            yield break;
        }

        public override string GetDescriptionText(RuntimeCard card, RuntimeCharacter playerCharacter)
        {
            return GetDescriptionText(value.ToString());
        }

        public override string GetDescriptionText()
        {
            return GetDescriptionText(value.ToString());
        }

        protected override string GetDescriptionText(string value)
        {
            return $"Gain {value} shield.";
        }
    }
}