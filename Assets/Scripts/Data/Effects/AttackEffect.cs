using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NaughtyAttributes;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Attack Effect", fileName = "New Attack Effect")]
    public class AttackEffect : EffectData
    {
        [Header("Target")] public EffectTarget effectTarget;

        [Header("Damage")] public ValueSource damageValueSource;

        [ShowIf("damageValueSource", ValueSource.CARD)]
        public int damageValue;

        [ShowIf("damageValueSource", ValueSource.CUSTOM)]
        public CustomValueSource customDamageValue;

        [ResizableTextArea] [ShowIf("damageValueSource", ValueSource.CUSTOM)]
        public string customDamageDescription;

        public override IEnumerator Execute(RuntimeCard card, RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            List<RuntimeCharacter> targets = new();

            // Get the affected targets for the size change effect
            switch (effectTarget)
            {
                case EffectTarget.NONE:
                    break;
                case EffectTarget.PLAYER:
                    targets.Add(player);
                    break;
                case EffectTarget.CARD_PLAYER:
                    targets.Add(characterPlayingTheCard);
                    break;
                case EffectTarget.TARGET:
                    targets.Add(cardTarget);
                    break;
                case EffectTarget.ALL_ENEMIES:
                    targets.AddRange(enemies);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Process the attack effect to every target
            int damage = GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies);
            int times = GetTimesValue(card, characterPlayingTheCard, player, cardTarget, enemies);

            for (int i = 0; i < times; i++)
            {
                // Process the attack to every target
                foreach (RuntimeCharacter target in targets)
                {
                    if (target.properties.Get<int>(PropertyKey.EVASION).Value > 0)
                    {
                        yield return Evade(target);
                    }
                    // A target might die during a multi-damage effect, so let's make sure we only attack targets that are ALIVE
                    else if (target.properties.Get<CharacterState>(PropertyKey.CHARACTER_STATE).Value == CharacterState.ALIVE)
                    {
                        yield return Attack(target, damage, characterPlayingTheCard, player, cardTarget, enemies, false);
                    }
                }
            }
        }

        public override string GetDescriptionTextWithModifiers(RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget,
            List<RuntimeCharacter> enemies)
        {
            StringBuilder sb = new();

            switch (effectTarget)
            {
                case EffectTarget.NONE:
                    throw new NotSupportedException();
                case EffectTarget.PLAYER:
                    throw new NotSupportedException();
                case EffectTarget.CARD_PLAYER:
                    throw new NotSupportedException();
                case EffectTarget.TARGET:
                    switch (damageValueSource)
                    {
                        case ValueSource.NONE:
                            throw new NotSupportedException();
                        case ValueSource.CARD:
                            sb.Append(
                                $"Deal {GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()} damage");
                            break;
                        case ValueSource.CUSTOM:
                            sb.Append(customDamageDescription);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case EffectTarget.ALL_ENEMIES:
                    switch (damageValueSource)
                    {
                        case ValueSource.NONE:
                            throw new NotSupportedException();
                        case ValueSource.CARD:
                            sb.Append(
                                $"Deal {GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()} damage to all enemies");
                            break;
                        case ValueSource.CUSTOM:
                            sb.Append(customDamageDescription);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (timesValueSource)
            {
                case ValueSource.NONE:
                    break;
                case ValueSource.CARD:
                    sb.Append(
                        $" {GetTimesValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()} times");
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

            switch (effectTarget)
            {
                case EffectTarget.NONE:
                    throw new NotSupportedException();
                case EffectTarget.PLAYER:
                    throw new NotSupportedException();
                case EffectTarget.CARD_PLAYER:
                    throw new NotSupportedException();
                case EffectTarget.TARGET:
                    switch (damageValueSource)
                    {
                        case ValueSource.NONE:
                            throw new NotSupportedException();
                        case ValueSource.CARD:
                            sb.Append($"Deal {GetEffectValue()} damage");
                            break;
                        case ValueSource.CUSTOM:
                            sb.Append(customDamageDescription);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case EffectTarget.ALL_ENEMIES:
                    switch (damageValueSource)
                    {
                        case ValueSource.NONE:
                            throw new NotSupportedException();
                        case ValueSource.CARD:
                            sb.Append($"Deal {GetEffectValue()} damage to all enemies");
                            break;
                        case ValueSource.CUSTOM:
                            sb.Append(customDamageDescription);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

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

        private IEnumerator Evade(RuntimeCharacter target)
        {
            target.Character.PlayParticle(FXKey.EVADE);
            target.properties.Get<int>(PropertyKey.EVASION).Value = Mathf.Clamp(target.properties.Get<int>(PropertyKey.EVASION).Value - 1, 0, int.MaxValue);
            yield break;
        }

        private IEnumerator Attack(RuntimeCharacter target, int incomingDamage,
            RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget,
            List<RuntimeCharacter> enemies, bool isThorn)
        {
            Property<int> shield = target.properties.Get<int>(PropertyKey.SHIELD);
            Property<int> health = target.properties.Get<int>(PropertyKey.HEALTH);
            Property<int> maxHealth = target.properties.Get<int>(PropertyKey.MAX_HEALTH);
            
            // Keep track of how much health the target had before receiving damage
            int healthBefore = health.Value;

            int vulnerable = target.properties.Get<int>(PropertyKey.VULNERABLE).GetValueWithModifiers(target);
            int weak = characterPlayingTheCard.properties.Get<int>(PropertyKey.WEAK).GetValueWithModifiers(characterPlayingTheCard);

            //Amp damage by 50% if target have VULNERABLE
            float damageAmp = vulnerable > 0 ? 0.5f : 0f;
            //Reduce damage by 25% if character perform attack have WEAK
            float damageReduc = weak > 0 ? 0.25f : 0f;
            
            float damageMod = (1 + damageAmp) * (1 - damageReduc);
            
            //Modify damage before calculate shield
            incomingDamage = (int)Mathf.Round(incomingDamage * damageMod);
            
            // Calculate the attack value after shield absorption (i.e. reduce shield value from attack value)
            int damageAbsorbedByShield = Mathf.Min(incomingDamage, shield.Value);
            
            int damage = incomingDamage - damageAbsorbedByShield;

            //Play animation/vfx if isn't thorn effect
            if (!isThorn)
            {
                yield return characterPlayingTheCard.Character.PlayAttackFeedback(target.Character.FrontPos);
            }

            // Reduce the absorbed attack value from the shield
            shield.Value = Mathf.Max(shield.Value - damageAbsorbedByShield, 0);

            // Reduce the final attack value from the target's health
            health.Value = Mathf.Clamp(health.Value - damage, 0, maxHealth.GetValueWithModifiers(target));

            // If target has thorns, deal damage from thorns to the attacker
            int thornsDamage = target.properties.Get<int>(PropertyKey.THORNS).GetValueWithModifiers(target);
            if (thornsDamage > 0 && !isThorn)
            {
                yield return Attack(characterPlayingTheCard, thornsDamage, target, player, characterPlayingTheCard,
                    enemies, true);
            }

            if (health.Value > 0)
            {
                if (healthBefore != health.Value)
                {
                    // If the target didn't die but their health changed -> trigger ON_HEALTH_CHANGED game event
                    yield return BattleManager.current.OnGameEvent(GameEvent.ON_HEALTH_CHANGED, target, player,
                        enemies);

                    //Damaged animation/vfx will be bind with properties.OnChanged
                }
            }
            else
            {
                yield return BattleManager.current.Kill(target, characterPlayingTheCard, player, cardTarget, enemies, FXKey.DEATH);
            }
        }

        /// <summary>
        /// Get the damage value inside a battle. Calculates the final value with all the modifiers.
        /// </summary>
        public override int GetEffectValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            int damage = damageValueSource switch
            {
                ValueSource.NONE => throw new NotSupportedException(),
                ValueSource.CARD => damageValue,
                ValueSource.CUSTOM => customDamageValue.GetValue(card, characterPlayingTheCard, player, cardTarget,
                    enemies),
                _ => throw new ArgumentOutOfRangeException()
            };

            int playerAttackModifier = characterPlayingTheCard.properties.Get<int>(PropertyKey.ATTACK)
                .GetValueWithModifiers(characterPlayingTheCard);
            int playerStrengthModifier = characterPlayingTheCard.properties.Get<int>(PropertyKey.STRENGTH)
                .GetValueWithModifiers(characterPlayingTheCard);
            int cardAttackModifier = card.properties.Get<int>(PropertyKey.ATTACK)
                .GetValueWithModifiers(characterPlayingTheCard);

            return damage + playerAttackModifier + playerStrengthModifier + cardAttackModifier;
        }


        /// <summary>
        /// Get damage value outside the battle. If you have a reference to the card instance
        /// the method will also calculate the card upgrades into the final value.
        /// </summary>
        public override string GetEffectValue(RuntimeCard card = null)
        {
            if (card == null)
            {
                return damageValueSource switch
                {
                    ValueSource.NONE => throw new NotSupportedException(),
                    ValueSource.CARD => damageValue.ToString(),
                    ValueSource.CUSTOM => "X",
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            return damageValueSource switch
            {
                ValueSource.NONE => throw new NotSupportedException(),
                ValueSource.CARD => (damageValue +
                                     card.properties.Get<int>(PropertyKey.ATTACK).GetValueWithModifiers(card))
                    .ToString(),
                ValueSource.CUSTOM => "X",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public override void PreviewEffect(RuntimeCard card, RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player,
            RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            List<RuntimeCharacter> targets = new();
            switch (effectTarget)
            {
                case EffectTarget.NONE:
                    break;
                case EffectTarget.PLAYER:
                    targets.Add(player);
                    break;
                case EffectTarget.CARD_PLAYER:
                    targets.Add(characterPlayingTheCard);
                    break;
                case EffectTarget.TARGET:
                    targets.Add(cardTarget);
                    break;
                case EffectTarget.ALL_ENEMIES:
                    targets.AddRange(enemies);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            int damage = GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies);
            int times = GetTimesValue(card, characterPlayingTheCard, player, cardTarget, enemies);

            for (int i = 0; i < times; i++)
            {
                foreach (var runtimeTarget in targets)
                {
                    var hp = runtimeTarget.properties.Get<int>(PropertyKey.HEALTH).Value;
                    var maxHp = runtimeTarget.properties.Get<int>(PropertyKey.MAX_HEALTH).Value;
                    var shield = runtimeTarget.properties.Get<int>(PropertyKey.SHIELD).Value;
                    int vulnerable = runtimeTarget.properties.Get<int>(PropertyKey.VULNERABLE).GetValueWithModifiers(cardTarget);
                    int weak = characterPlayingTheCard.properties.Get<int>(PropertyKey.WEAK).GetValueWithModifiers(characterPlayingTheCard);

                    //Amp damage by 50% if target have VULNERABLE
                    float damageAmp = vulnerable > 0 ? 0.5f : 0f;
                    //Reduce damage by 25% if character perform attack have WEAK
                    float damageReduc = weak > 0 ? 0.25f : 0f;
            
                    float damageMod = (1 + damageAmp) * (1 - damageReduc);
                    damage = (int)Mathf.Round(damage * damageMod);
                    
                    if (shield > 0)
                    {
                        runtimeTarget.Character.statUI.PreviewShield(shield, shield - damage);
                    }

                    damage -= shield;

                    runtimeTarget.Character.statUI.PreviewHp(hp, hp - damage, maxHp);
                }
            }
        }
    }
}