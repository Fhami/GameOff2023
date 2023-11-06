using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Modify Property Effect", fileName = "New Modify Property Effect")]
    public class ModifyPropertyEffect : EffectData
    {
        public Operation operation;
        public PropertyKey propertyKey;
        public int value;
        
        public override IEnumerator Execute(RuntimeCard card, RuntimeCharacter player, RuntimeCharacter target, List<RuntimeCharacter> enemies)
        {
            throw new System.NotImplementedException();
        }
    }
}