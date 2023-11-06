using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Draw Card Effect", fileName = "New Draw Card Effect")]
    public class DrawCardEffect : EffectData
    {
        public int count;
        
        public override IEnumerator Execute(RuntimeCard card, RuntimeCharacter player, RuntimeCharacter target, List<RuntimeCharacter> enemies)
        {
            // TODO: Draw card (visuals + data)
            
            // TODO: Execute passive/active skills that trigger on CardEvent.CARD_DRAWN
            
            throw new System.NotImplementedException();
        }

        public override string GetDescriptionText(RuntimeCard card, RuntimeCharacter player)
        {
            return $"DRAW {count}";
        }
    }
}