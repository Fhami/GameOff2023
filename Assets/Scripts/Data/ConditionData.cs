using UnityEngine;

namespace DefaultNamespace
{
    public abstract class ConditionData : ScriptableObject
    {
        public abstract bool Evaluate(GameEvent gameEvent, RuntimeCharacter player, RuntimeCharacter target);
    }
}