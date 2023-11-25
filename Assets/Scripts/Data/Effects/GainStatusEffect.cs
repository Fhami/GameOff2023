using System.Collections;
using System.Collections.Generic;
using RoboRyanTron.SearchableEnum;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/GainStatusEffect")]
    public class GainStatusEffect : EffectData
    {
        public int value = 1;
        
        [SearchableEnum]
        public PropertyKey statusEffect;
        
        public override IEnumerator Execute(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player,
            RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            characterPlayingTheCard.properties.Get<int>(statusEffect).Value += value;
            yield break;
        }

        public override string GetDescriptionTextWithModifiers(RuntimeCard card, RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            return $"Gain {value.ToString()} {statusEffect.ToString()}.";
        }

        public override string GetDescriptionText()
        {
            return $"Gain {value.ToString()} {statusEffect.ToString()}.";
        }

        public override int GetEffectValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player,
            RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            return value;
        }

        public override string GetEffectValue(RuntimeCard card = null)
        {
            return value.ToString();
        }

        public override int GetTimesValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player,
            RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            return 1;
        }

        public override string GetTimesValue(RuntimeCard card = null)
        {
            return "1";
        }
    }
}