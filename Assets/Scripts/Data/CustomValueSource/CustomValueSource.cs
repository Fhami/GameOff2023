using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public abstract class CustomValueSource : ScriptableObject
    {
        public abstract int GetValue(RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player,
            RuntimeCharacter cardTarget,
            List<RuntimeCharacter> enemies);
        
        public abstract string GetDescription();
    }
}