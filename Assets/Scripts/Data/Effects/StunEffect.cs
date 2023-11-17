using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Stun Effect", fileName = "New Stun Effect")]
    public class StunEffect : EffectData
    {
        public EffectTarget effectTarget;
        public int value;
        
        public override IEnumerator Execute(
            RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player,
            RuntimeCharacter cardTarget,
            List<RuntimeCharacter> enemies)
        {
            // TODO: What if player stuns themselves on their own turn? Then we need to disable the ability to play cards.
            switch (effectTarget)
            {
                case EffectTarget.NONE: throw new NotSupportedException();
                case EffectTarget.PLAYER: throw new NotSupportedException();
                case EffectTarget.CARD_PLAYER:
                {
                    ApplyStun(characterPlayingTheCard);
                    break;
                }
                case EffectTarget.TARGET:
                {
                    ApplyStun(cardTarget);
                    break;
                }
                case EffectTarget.ALL_ENEMIES:
                {
                    foreach (RuntimeCharacter enemyCharacter in enemies)
                    {
                        ApplyStun(enemyCharacter);
                    }
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            yield break;
        }

        private void ApplyStun(RuntimeCharacter target)
        {
            // TODO: VFX
            target.properties.Get<int>(PropertyKey.STUN).Value += value;
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
            switch (effectTarget)
            {
                case EffectTarget.NONE: throw new NotSupportedException();
                case EffectTarget.PLAYER: throw new NotSupportedException();
                case EffectTarget.CARD_PLAYER: return $"Apply {value.ToString()} stun to self.";
                case EffectTarget.TARGET: return $"Apply {value.ToString()} stun to target.";
                case EffectTarget.ALL_ENEMIES: return $"Apply {value.ToString()} stun to all enemies.";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}