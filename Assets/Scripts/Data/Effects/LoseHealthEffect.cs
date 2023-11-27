using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NaughtyAttributes;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Lose Health Effect", fileName = "New Lose Health Effect")]
    public class LoseHealthEffect : EffectData
    {
        [Header("Health")]
        public ValueSource healthValueSource;
        
        [ShowIf("healthValueSource", ValueSource.CARD)]
        public int healthValue;
        
        [ShowIf("healthValueSource", ValueSource.CUSTOM)]
        public CustomValueSource customHealthValue;
        
        [ResizableTextArea]
        [ShowIf("healthValueSource", ValueSource.CUSTOM)]
        public string customHealthDescription;
        
        public override IEnumerator Execute(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            //Lose health mean character that use this effect so we set target to characterPlayingTheCard
            cardTarget = characterPlayingTheCard;
            
            // The amount of health to lose
            int health = GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies);

            // TODO: VFX
            int times = GetTimesValue(card, characterPlayingTheCard, player, cardTarget, enemies);
            // Process the effect
            for (int i = 0; i < times; i++)
            {
                yield return LoseHealth(cardTarget, health, characterPlayingTheCard, player, cardTarget, enemies);
            }
        }

        private IEnumerator LoseHealth(RuntimeCharacter target, int incomingHealthChange, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            Property<int> health = target.properties.Get<int>(PropertyKey.HEALTH);
            Property<int> maxHealth = target.properties.Get<int>(PropertyKey.MAX_HEALTH);

            // Keep track of how much health the target had before the health changed
            int healthBefore = health.Value;
            
            // Reduce the final attack value from the target's health
            health.Value = Mathf.Clamp(health.Value - incomingHealthChange, 0, maxHealth.GetValueWithModifiers(target));
            
            if (health.Value > 0)
            {
                if (healthBefore != health.Value)
                {
                    // If the target didn't die but their health changed -> trigger ON_HEALTH_CHANGED game event
                    yield return BattleManager.current.OnGameEvent(GameEvent.ON_HEALTH_CHANGED, target, player, enemies);
                }
            }
            else
            {
                yield return BattleManager.current.Kill(target, characterPlayingTheCard, player, cardTarget, enemies, FXKey.DEATH);
            }
        }

        public override string GetDescriptionTextWithModifiers(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            StringBuilder sb = new();
            
            switch (healthValueSource)
            {
                case ValueSource.NONE:
                    break;
                case ValueSource.CARD:
                    sb.Append($"Lose {GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()} health");
                    break;
                case ValueSource.CUSTOM:
                    sb.Append(" " + customHealthDescription);
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
            
            switch (healthValueSource)
            {
                case ValueSource.NONE:
                    break;
                case ValueSource.CARD:
                    sb.Append($"Lose {GetEffectValue()} health");
                    break;
                case ValueSource.CUSTOM:
                    sb.Append(" " + customHealthDescription);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            switch (timesValueSource)
            {
                case ValueSource.NONE:
                    break;
                case ValueSource.CARD:
                    sb.Append($" {GetTimesValue().ToString()} times");
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
        /// Get the health value inside a battle. Calculates the final value with all the modifiers.
        /// </summary>
        public override int GetEffectValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            int health = healthValueSource switch
            {
                ValueSource.NONE => throw new NotSupportedException(),
                ValueSource.CARD => healthValue,
                ValueSource.CUSTOM => customHealthValue.GetValue(card, characterPlayingTheCard, player, cardTarget, enemies),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            return health;
        }
        
        /// <summary>
        /// Get health value outside the battle. If you have a reference to the card instance
        /// the method will also calculate the card upgrades into the final value.
        /// </summary>
        public override string GetEffectValue(RuntimeCard card = null)
        {
            if (card == null)
            {
                return healthValueSource switch
                {
                    ValueSource.NONE => throw new NotSupportedException(),
                    ValueSource.CARD => healthValue.ToString(),
                    ValueSource.CUSTOM => "X",
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
          
            return healthValueSource switch
            {
                ValueSource.NONE => throw new NotSupportedException(),
                ValueSource.CARD => (healthValue + card.properties.Get<int>(PropertyKey.HEALTH).GetValueWithModifiers(card)).ToString(),
                ValueSource.CUSTOM => "X",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}