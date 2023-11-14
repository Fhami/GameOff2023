using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Evade Effect", fileName = "New Evade Effect")]
    public class EvadeEffect : EffectData
    {
        public int value;
        
        public override IEnumerator Execute(
            RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player,
            RuntimeCharacter cardTarget,
            List<RuntimeCharacter> enemies)
        {
            characterPlayingTheCard.properties.Get<int>(PropertyKey.EVADE).Value += value;
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
            return $"Gain {value} evade.";
        }
    }
}