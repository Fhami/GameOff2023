using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EPOOutline;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    // TODO: This can be attached to character prefab
    public class Character : MonoBehaviour, ICardTarget
    {
        public RuntimeCharacter runtimeCharacter;
        public CardController cardController;

        [SerializeField] private Outlinable outlinable;
        [SerializeField] private StatsUI statUI;
        [SerializeField] private SizeUI sizeUI;
        [SerializeField] private IntentionUI intentionUI;

        public void Init(RuntimeCharacter _runtimeCharacter)
        {
            runtimeCharacter = _runtimeCharacter;
            runtimeCharacter.Character = this;
            
            //Update visual
            UpdateHpVisual(0, runtimeCharacter.properties.Get<int>(PropertyKey.HEALTH));
            UpdateSizeVisual(0, runtimeCharacter.properties.Get<int>(PropertyKey.SIZE));
            UpdateShield(0, runtimeCharacter.properties.Get<int>(PropertyKey.SHIELD));
            UpdateFormVisual(runtimeCharacter.GetCurrentForm());
            
            runtimeCharacter.properties.Get<int>(PropertyKey.HEALTH).OnChanged += UpdateHpVisual;
            runtimeCharacter.properties.Get<int>(PropertyKey.SIZE).OnChanged += UpdateSizeVisual;
            runtimeCharacter.properties.Get<int>(PropertyKey.SHIELD).OnChanged += UpdateShield;
            
            outlinable.AddAllChildRenderersToRenderingList();
        }

        public GameObject GameObject => gameObject;

        public void Highlight(bool _value)
        {
            outlinable.enabled = _value;
        }

        public void HighlightSelected(bool _value)
        {
            if (_value)
                outlinable.OutlineParameters.FillPass.Shader =
                    Resources.Load<Shader>("Easy performant outline/Shaders/Fills/ColorFill");
            else
                outlinable.OutlineParameters.FillPass.Shader = null;
        }

        public void UpdateHpVisual(int _oldValue, Property<int> _value)
        {
            statUI.SetHp(_oldValue, _value.Value, runtimeCharacter.properties.Get<int>(PropertyKey.MAX_HEALTH).Value);
        }

        public void UpdateShield(int _oldValue, Property<int> _value)
        {
            statUI.SetShield(_oldValue,_value.Value);
        }

        public void UpdateSizeVisual(int _oldValue, Property<int> _size)
        {
            var _sizeEffect = _oldValue > _size.Value ? SizeEffectType.Increase : SizeEffectType.Decrease;
            sizeUI.SetSize(_size.Value, _sizeEffect);
        }

        public IEnumerator UpdateIntention(RuntimeCard _runtimeCard)
        {
            var _intentDetails = new List<IntentionDetail>();

            var _battleManager = BattleManager.current;
            
            foreach (var _effectData in _runtimeCard.cardData.effects)
            {
                var _value = _effectData.GetEffectValue(_runtimeCard, runtimeCharacter, _battleManager.runtimePlayer,
                    _battleManager.runtimePlayer, _battleManager.runtimeEnemies);

                var _times = _effectData.GetTimesValue(_runtimeCard, runtimeCharacter, _battleManager.runtimePlayer,
                    _battleManager.runtimePlayer, _battleManager.runtimeEnemies);

                var _description = _effectData.GetDescriptionTextWithModifiers(_runtimeCard, runtimeCharacter,
                    _battleManager.runtimePlayer, _battleManager.runtimePlayer, _battleManager.runtimeEnemies);

                _intentDetails.Add(new IntentionDetail(_effectData.intent, _value, _times, _description));
            }
            
            yield return intentionUI.SetIntention(_intentDetails);
        }

        public void UpdateFormVisual(FormData _form)
        {
            
        }

        /// <summary>
        /// Get current enemy's intention
        /// </summary>
        public RuntimeCard GetIntention()
        {
            FormData _form = runtimeCharacter.GetCurrentForm();
            Property<int> _cardIndex = runtimeCharacter.properties.Get<int>(PropertyKey.ENEMY_ATTACK_PATTERN_CARD_INDEX);
            CardData _cardData = _form.attackPattern[_cardIndex.Value];
            RuntimeCard _card = CardFactory.Create(_cardData);

            return _card;
        }
    }
}