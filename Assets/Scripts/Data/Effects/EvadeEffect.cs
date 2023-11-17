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

        public override string GetDescriptionTextWithModifiers(RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player,
            RuntimeCharacter cardTarget,
            List<RuntimeCharacter> enemies)
        {
            return GetDescriptionText();
        }

        public override string GetDescriptionText()
        {
            return $"Gain {value.ToString()} evade.";
        }
    }
}