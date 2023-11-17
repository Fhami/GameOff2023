using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public enum ValueSource
    {
        NONE,
        CARD,
        CUSTOM
    }
    
    public abstract class CustomValueSource : ScriptableObject
    {
        public abstract int GetValue(RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player,
            RuntimeCharacter cardTarget,
            List<RuntimeCharacter> enemies);

        public string GetDescription()
        {
            return name;
        }
    }
}