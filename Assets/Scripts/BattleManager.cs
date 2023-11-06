using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// Main manager used in BattleScene.
    /// </summary>
    public class BattleManager : MonoBehaviour
    {
        private void Awake()
        {
            Debug.Log("Hello from BattleManager!");
        }

        /// <summary>
        /// Executes each effect in the card in order.
        /// </summary>
        /// <param name="card">The card instance.</param>
        /// <param name="player">The player character.</param>
        /// <param name="dragTarget">The character we dragged this card on top of. This can be player or enemy.</param>
        /// <param name="enemies">List of all enemies in the current battle.</param>
        public void ExecuteCardEffects(
            RuntimeCard card,
            RuntimeCharacter player,
            RuntimeCharacter dragTarget,
            List<RuntimeCharacter> enemies)
        {
            foreach (Effect effect in card.cardData.effects)
            {
                switch (effect.effectTarget)
                {
                    case EffectTarget.NONE:
                        break;
                    case EffectTarget.DRAG_TARGET:
                        ApplyEffectToTarget(effect, dragTarget);
                        break;
                    case EffectTarget.ALL_ENEMIES:
                        foreach (RuntimeCharacter target in enemies)
                        {
                            ApplyEffectToTarget(effect, target);
                        }
                        break;
                    case EffectTarget.PLAYER:
                        ApplyEffectToTarget(effect, player);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Applies a single effect to a target.
        /// </summary>
        /// <param name="effect">The effect to apply.</param>
        /// <param name="target">The target which the effect is applied to.</param>
        private void ApplyEffectToTarget(Effect effect, RuntimeCharacter target)
        {
            IProperty property = target.properties.Get(effect.propertyKey);

            switch (property)
            {
                case Property<int> intProperty:
                    intProperty.ApplyOperation(effect.operation, (int)effect.value);
                    break;
            }
        }
    }
}

