using UnityEngine;

namespace DefaultNamespace
{
    public enum BuffType
    {
        NONE,
        Buff,
        Debuff
    }
    
    public abstract class BuffData : ScriptableObject
    {
        public PropertyKey buffPropertyKey;
        public BuffType type;
        public Sprite icon;

        public abstract string GetDescription(RuntimeCharacter character);
    }
}