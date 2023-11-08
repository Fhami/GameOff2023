using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Draw Card Effect", fileName = "New Draw Card Effect")]
    public class DrawCardEffect : EffectData
    {
        public int count;
        
        public override IEnumerator Execute(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter playerCharacter, RuntimeCharacter targetCharacter, List<RuntimeCharacter> enemyCharacters)
        {
            // TODO: VFX
            // TODO: Draw card from draw pile and put it to hand
            // TODO: Execute passive/active skills that trigger on GameEvent.CARD_DRAWN
            throw new System.NotImplementedException();
        }

        public override string GetDescriptionText(RuntimeCard card, RuntimeCharacter playerCharacter)
        {
            return $"Draw {count} cards.";
        }
    }
}