using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Draw Card Effect", fileName = "New Draw Card Effect")]
    public class DrawCardEffect : EffectData
    {
        public int count;

        public override IEnumerator Execute(
            RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter playerCharacter,
            RuntimeCharacter targetCharacter,
            List<RuntimeCharacter> enemyCharacters)
        {
            if (characterPlayingTheCard.properties.Get<bool>(PropertyKey.CANNOT_DRAW_ADDITIONAL_CARDS_CURRENT_TURN).Value)
            {
                // TODO: If we want we can make VFX + animation for what happens when we can't draw any more cards but are trying to
                // TODO: I don't remember what Slay the Spire does..
                yield break;
            }
            
            throw new NotImplementedException("TODO: Draw card!");
            // TODO: VFX
            // TODO: Draw card from draw pile and put it to hand
            // TODO: Execute passive/active skills that trigger on GameEvent.CARD_DRAWN
        }

        public override string GetDescriptionText(RuntimeCard card, RuntimeCharacter playerCharacter)
        {
            return $"Draw {count} cards.";
        }
    }
}