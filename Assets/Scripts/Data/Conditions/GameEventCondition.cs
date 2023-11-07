using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Conditions/GameEvent Condition", fileName = "New GameEvent Condition")]
    public class GameEventCondition : ConditionData
    {
        public GameEvent gameEvent;
        
        public override bool Evaluate(GameEvent gameEvent, RuntimeCharacter player, RuntimeCharacter target)
        {
            // GameEvent must match for this condition to success
            return this.gameEvent == gameEvent;
        }
    }
}