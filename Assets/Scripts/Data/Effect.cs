using System;

namespace DefaultNamespace
{
    /// <summary>
    /// Determines what target the effect is applied to.
    /// </summary>
    public enum EffectTarget
    {
        NONE,
        DRAG_TARGET,
        ALL_ENEMIES,
        PLAYER
    }

    /// <summary>
    /// What type of effect.
    /// </summary>
    public enum EffectType
    {
        NONE,
        ATTACK,
        SIZE,
        HEAL
    }
    
    /// <summary>
    /// Represents a single effect.
    /// </summary>
    [Serializable]
    public class Effect
    {
        public EffectTarget effectTarget;
        public EffectType effectType;
        public int value;
    }
}