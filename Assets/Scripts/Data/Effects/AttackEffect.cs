using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Attack Effect", fileName = "New Attack Effect")]
    public class AttackEffect : EffectData
    {
        public int damage;
        public bool aoe;
        
        public override IEnumerator Execute(RuntimeCard card, RuntimeCharacter player, RuntimeCharacter target, List<RuntimeCharacter> enemies)
        {
            // TODO: Deal damage (visuals + effect)
            
            // Attack multiple targets
            if (aoe)
            {
                foreach (RuntimeCharacter enemy in enemies)
                {
                    Property<int> targetHealth = enemy.properties.Get<int>(PropertyKey.HEALTH);
                    targetHealth.Value = Mathf.Clamp(targetHealth.Value - damage, 0, int.MaxValue);
                }
            }
            // Attack a single target
            else
            {
                Property<int> targetHealth = target.properties.Get<int>(PropertyKey.HEALTH);
                targetHealth.Value = Mathf.Clamp(targetHealth.Value - damage, 0, int.MaxValue);
            }
            
            throw new System.NotImplementedException();
        }
    }
}