using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NaughtyAttributes;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Gain Thorns Effect", fileName = "New Gain Thorns Effect")]
    public class GainThornsEffect : EffectData
    {
        [Header("Thorns")]
        public ValueSource thornsValueSource;
        
        [ShowIf("thornsValueSource", ValueSource.CARD)]
        public int thornsValue;
        
        [ShowIf("thornsValueSource", ValueSource.CUSTOM)]
        public CustomValueSource customThornsValue;
        
        [ResizableTextArea]
        [ShowIf("thornsValueSource", ValueSource.CUSTOM)]
        public string customThornsDescription;
        
        public override IEnumerator Execute(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            // TODO: VFX
            
            int thorns = GetThornsValue(card, characterPlayingTheCard, player, cardTarget, enemies);
            
            characterPlayingTheCard.properties.Get<int>(PropertyKey.THORNS).Value += thorns;
            
            yield break;
        }

        public override string GetDescriptionTextWithModifiers(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            StringBuilder sb = new();
            
            switch (thornsValueSource)
            {
                case ValueSource.NONE:
                    break;
                case ValueSource.CARD:
                    sb.Append($"Gain {GetThornsValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()} thorns");
                    break;
                case ValueSource.CUSTOM:
                    sb.Append(" " + customThornsDescription);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            sb.Append(".");

            return sb.ToString();
        }

        public override string GetDescriptionText()
        {
            StringBuilder sb = new();
            
            switch (thornsValueSource)
            {
                case ValueSource.NONE:
                    break;
                case ValueSource.CARD:
                    sb.Append($"Gain {GetThornsValue()} thorns");
                    break;
                case ValueSource.CUSTOM:
                    sb.Append(" " + customThornsDescription);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            sb.Append(".");

            return sb.ToString();
        }
        
        /// <summary>
        /// Get the thorns value inside a battle. Calculates the final value with all the modifiers.
        /// </summary>
        public int GetThornsValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            int damage = thornsValueSource switch
            {
                ValueSource.NONE => throw new NotSupportedException(),
                ValueSource.CARD => thornsValue,
                ValueSource.CUSTOM => customThornsValue.GetValue(card, characterPlayingTheCard, player, cardTarget, enemies),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            int cardThornsModifier = card.properties.Get<int>(PropertyKey.THORNS).GetValueWithModifiers(card);
            
            return damage + cardThornsModifier;
        }
        
        /// <summary>
        /// Get thorns value outside the battle. If you have a reference to the card instance
        /// the method will also calculate the card upgrades into the final value.
        /// </summary>
        public string GetThornsValue(RuntimeCard card = null)
        {
            if (card == null)
            {
                return thornsValueSource switch
                {
                    ValueSource.NONE => throw new NotSupportedException(),
                    ValueSource.CARD => thornsValue.ToString(),
                    ValueSource.CUSTOM => "X",
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
          
            return thornsValueSource switch
            {
                ValueSource.NONE => throw new NotSupportedException(),
                ValueSource.CARD => (thornsValue + card.properties.Get<int>(PropertyKey.THORNS).GetValueWithModifiers(card)).ToString(),
                ValueSource.CUSTOM => "X",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}