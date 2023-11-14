using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Cannot Draw Additional Cards Effect", fileName = "New Cannot Draw Additional Cards Effect")]
    public class CannotDrawAdditionalCardsEffect : EffectData
    {
        public override IEnumerator Execute(
            RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player,
            RuntimeCharacter cardTarget,
            List<RuntimeCharacter> enemies)
        {
            characterPlayingTheCard.properties.Get<bool>(PropertyKey.CANNOT_DRAW_ADDITIONAL_CARDS_CURRENT_TURN).Value = true;
            yield break;
        }

        public override string GetDescriptionTextWithModifier(RuntimeCard card, RuntimeCharacter playerCharacter)
        {
            return GetDescriptionText("");
        }

        public override string GetDescriptionText()
        {
            return GetDescriptionText(""); 
        }

        protected override string GetDescriptionText(string value)
        {
            return $"You cannot draw any additional cards this turn."; 
        }
    }
}