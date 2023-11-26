using System.Text;

namespace DefaultNamespace
{
    public enum CardState
    {
        NONE,
        PLAYING,
        DRAW_PILE,
        HAND,
        DISCARD_PILE,
        AVAILABLE, //For player active skill
        FADED,
        DESTROYED
    }
    
    /// <summary>
    /// The runtime instance of a card. This can be modified during runtime.
    /// </summary>
    public class RuntimeCard : RuntimeEntity
    {
        /// <summary>
        /// The base data of the card.
        /// </summary>
        public CardData cardData;

        public CardState cardState;

        public Card Card;

        public static string GetCardDescriptionWithModifiers(RuntimeCharacter _character, RuntimeCard _runtimeCard)
        {
            StringBuilder _builder = new StringBuilder();
            foreach (var _effect in _runtimeCard.cardData.effects)
            {
                var _description = _effect.GetDescriptionTextWithModifiers(_runtimeCard, _character, _character, null,
                    BattleManager.current.runtimeEnemies);
                
                _builder.AppendLine(_description);
                if (_effect.effectModifier)
                {
                    _builder.AppendLine(_effect.effectModifier.name);
                }
                
                //Debug.Log($"{_effect.name} {_description}");
            }

            foreach (var _skill in _runtimeCard.cardData.cardActiveSkills)
            {
                _builder.AppendLine(_skill.name);
            }

            return _builder.ToString();
        }

        public bool IsPersist()
        {
            foreach (var _effect in cardData.effects)
            {
                if (_effect.GetType() == typeof(PersistEffect))
                {
                    return true;
                }
            }

            return false;
        }
    }
}