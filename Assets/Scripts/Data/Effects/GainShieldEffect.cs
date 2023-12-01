using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NaughtyAttributes;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Gain Shield Effect", fileName = "New Gain Shield Effect")]
    public class GainShieldEffect : EffectData
    {
        [Header("Shield")]
        public ValueSource shieldValueSource;
        
        [ShowIf("shieldValueSource", ValueSource.CARD)]
        public int shieldValue;
        
        [ShowIf("shieldValueSource", ValueSource.CUSTOM)]
        public CustomValueSource customShieldValue;
        
        [ResizableTextArea]
        [ShowIf("shieldValueSource", ValueSource.CUSTOM)]
        public string customShieldDescription;
        
        public override IEnumerator Execute(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            // TODO: VFX
            
            int shield = GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies);
            var times = GetTimesValue(card, characterPlayingTheCard, player, cardTarget, enemies);

            var fragile = characterPlayingTheCard.properties.Get<int>(PropertyKey.FRAGILE)
                .GetValueWithModifiers(characterPlayingTheCard);
            
            //Reduce shield gain by 25% if character have FRAGILE
            var shieldReduc = fragile > 0 ? 0.25f : 0;
            var shieldMod = (1 - shieldReduc);
            
            shield = (int)Mathf.Round(shield * shieldMod);
            
            for (int i = 0; i < times; i++)
            {
                characterPlayingTheCard.properties.Get<int>(PropertyKey.SHIELD).Value += shield;
            }
            
            yield return new WaitForSeconds(0.2f);
        }

        public override string GetDescriptionTextWithModifiers(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            StringBuilder sb = new();
            
            switch (shieldValueSource)
            {
                case ValueSource.NONE:
                    break;
                case ValueSource.CARD:
                    sb.Append($"Gain {GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()} <color={Colors.COLOR_STATUS}>Shield</color>");
                    break;
                case ValueSource.CUSTOM:
                    sb.Append(" " + customShieldDescription);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            switch (timesValueSource)
            {
                case ValueSource.NONE:
                    break;
                case ValueSource.CARD:
                    sb.Append($" {GetTimesValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()} times");
                    break;
                case ValueSource.CUSTOM:
                    sb.Append(" " + customTimesDescription);
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
            
            switch (shieldValueSource)
            {
                case ValueSource.NONE:
                    break;
                case ValueSource.CARD:
                    sb.Append($"Gain {GetEffectValue()} <color={Colors.COLOR_STATUS}>Shield</color>");
                    break;
                case ValueSource.CUSTOM:
                    sb.Append(" " + customShieldDescription);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            switch (timesValueSource)
            {
                case ValueSource.NONE:
                    break;
                case ValueSource.CARD:
                    sb.Append($" {GetTimesValue()} times");
                    break;
                case ValueSource.CUSTOM:
                    sb.Append(" " + customTimesDescription);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            sb.Append(".");

            return sb.ToString();
        }
        
        /// <summary>
        /// Get the shield value inside a battle. Calculates the final value with all the modifiers.
        /// </summary>
        public override int GetEffectValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            int shield = shieldValueSource switch
            {
                ValueSource.NONE => throw new NotSupportedException(),
                ValueSource.CARD => shieldValue,
                ValueSource.CUSTOM => customShieldValue.GetValue(card, characterPlayingTheCard, player, cardTarget, enemies),
                _ => throw new ArgumentOutOfRangeException()
            };

            int cardShieldModifier = GetShieldModifiers(card, characterPlayingTheCard);

            return shield + cardShieldModifier;
        }
        
        /// <summary>
        /// Get shield value outside the battle. If you have a reference to the card instance
        /// the method will also calculate the card upgrades into the final value.
        /// </summary>
        public override string GetEffectValue(RuntimeCard card = null)
        {
            if (card == null)
            {
                return shieldValueSource switch
                {
                    ValueSource.NONE => throw new NotSupportedException(),
                    ValueSource.CARD => shieldValue.ToString(),
                    ValueSource.CUSTOM => "X",
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
          
            return shieldValueSource switch
            {
                ValueSource.NONE => throw new NotSupportedException(),
                ValueSource.CARD => (shieldValue + GetShieldModifiers(card, null)).ToString(),
                ValueSource.CUSTOM => "X",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private int GetShieldModifiers(RuntimeCard card, RuntimeCharacter characterPlayingTheCard)
        {
            int cardShieldModifier = card.properties.Get<int>(PropertyKey.SHIELD).GetValueWithModifiers(card);
            var shieldUpModifier = characterPlayingTheCard != null ? characterPlayingTheCard.properties.Get<int>(PropertyKey.SHIELD_UP).GetValueWithModifiers(card) : 0;
            var shieldDownModifier = characterPlayingTheCard != null ? characterPlayingTheCard.properties.Get<int>(PropertyKey.SHIELD_DOWN).GetValueWithModifiers(card) : 0;

            return cardShieldModifier + shieldUpModifier - shieldDownModifier;
        }

        public override void PreviewEffect(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player,
            RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            int shield = GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies);
            var times = GetTimesValue(card, characterPlayingTheCard, player, cardTarget, enemies);
            var currentShield = characterPlayingTheCard.properties.Get<int>(PropertyKey.SHIELD).Value;
            
            var fragile = characterPlayingTheCard.properties.Get<int>(PropertyKey.FRAGILE)
                .GetValueWithModifiers(characterPlayingTheCard);
            
            //Reduce shield gain by 25% if character have FRAGILE
            var shieldReduc = fragile > 0 ? 0.25f : 0;
            var shieldMod = (1 - shieldReduc);
            
            shield = (int)Mathf.Round(shield * shieldMod);
            
            for (int i = 0; i < times; i++)
            {
                characterPlayingTheCard.Character.statUI.PreviewShield(currentShield, currentShield + shield);
            }
        }
    }
}