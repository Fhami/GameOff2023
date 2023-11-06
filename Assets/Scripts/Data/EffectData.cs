using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public abstract class EffectData : ScriptableObject
    {
        public abstract IEnumerator Execute(RuntimeCard card, RuntimeCharacter player, RuntimeCharacter target, List<RuntimeCharacter> enemies);
    }
}