using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Unplayable Effect", fileName = "New Unplayable Effect")]
    public class UnplayableEffect : EffectData
    {
        public override IEnumerator Execute(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            yield break;
        }

        public override string GetDescriptionTextWithModifiers(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            return GetDescriptionText();
        }

        public override string GetDescriptionText()
        {
            return "Unplayable.";
        }
    }
}