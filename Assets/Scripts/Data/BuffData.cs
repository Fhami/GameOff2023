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

        public abstract string GetDescriptionWithModifier(RuntimeCharacter character);
        public abstract string GetDescription(int value);
    }
}