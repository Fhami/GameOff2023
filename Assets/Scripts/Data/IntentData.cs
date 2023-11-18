using UnityEngine;

namespace DefaultNamespace
{
    public enum IntentType
    {
        NONE,
        Attack,
        StrongAttack,
        Shield,
        Evade,
        SizeUp,
        SizeDown,
        SetSize,
        TargetSizeUp,
        TargetSizeDown,
        SetTargetSize,
        Heal,
        Buff,
        Stun,
        Debuff,
        Unknown
    }
    
    /// <summary>
    /// Contains data about the intents. The derived classes are empty for now
    /// but we can add some custom logic there per intent if we need to! (like we do in BuffData)
    /// </summary>
    public abstract class IntentData : ScriptableObject
    {
        public IntentType intentType;
        public Sprite icon;
    }
}