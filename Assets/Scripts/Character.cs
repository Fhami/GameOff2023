using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using EPOOutline;
using NaughtyAttributes;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    // TODO: This can be attached to character prefab
    public class Character : MonoBehaviour, ICardTarget
    {
        public RuntimeCharacter runtimeCharacter;
        public CardController cardController;

        [SerializeField] private SerializedDictionary<FormData, CharacterForm> characterForms;
        [SerializeField] private CharacterForm currentForm;

        [SerializeField] private Outlinable outlinable;
        [BoxGroup("UI"), SerializeField] private StatsUI statUI;
        [BoxGroup("UI"), SerializeField] private SizeUI sizeUI;
        [BoxGroup("UI"), SerializeField] private IntentionUI intentionUI;

        [BoxGroup("Particle"), SerializeField] private ParticleSystem deathParticle;
        [BoxGroup("Particle"), SerializeField] private ParticleSystem healParticle;
        [BoxGroup("Particle"), SerializeField] private ParticleSystem damagedParticle;
        [BoxGroup("Particle"), SerializeField] private ParticleSystem attackParticle;
        
        public void Init(RuntimeCharacter _runtimeCharacter)
        {
            runtimeCharacter = _runtimeCharacter;
            runtimeCharacter.Character = this;
            
            //Hide all form first
            foreach (var _form in characterForms.Values)
            {
                _form.gameObject.SetActive(false);
            }
            
            //Update visual
            UpdateHpVisual(0, runtimeCharacter.properties.Get<int>(PropertyKey.HEALTH));
            UpdateSizeVisual(0, runtimeCharacter.properties.Get<int>(PropertyKey.SIZE));
            UpdateShield(0, runtimeCharacter.properties.Get<int>(PropertyKey.SHIELD));

            runtimeCharacter.properties.Get<int>(PropertyKey.HEALTH).OnChanged += UpdateHpVisual;
            runtimeCharacter.properties.Get<int>(PropertyKey.SIZE).OnChanged += UpdateSizeVisual;
            runtimeCharacter.properties.Get<int>(PropertyKey.SHIELD).OnChanged += UpdateShield;
        }

        public GameObject GameObject => gameObject;

        #region Visual

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

            if (_oldValue > _value.Value)
            {
                //Play animation
                if (damagedParticle)
                {
                    var _particle = Instantiate(damagedParticle);
                    _particle.transform.position = transform.position;
                }
            }
            else
            {
                //Play animation
                if (healParticle)
                {
                    var _particle = Instantiate(healParticle);
                    _particle.transform.position = transform.position;
                }
            }
        }

        public void UpdateShield(int _oldValue, Property<int> _value)
        {
            statUI.SetShield(_oldValue,_value.Value);
        }

        public void UpdateSizeVisual(int _oldValue, Property<int> _size)
        {
            var _sizeEffect = _oldValue > _size.Value ? SizeEffectType.Increase : SizeEffectType.Decrease;
            sizeUI.SetSize(_size.Value, _sizeEffect);
            
            UpdateFormVisual(runtimeCharacter.GetCurrentForm());
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
            if (characterForms.TryGetValue(_form, out var _characterForm))
            {
                if (currentForm == _characterForm) return;

                if (currentForm)
                    currentForm.gameObject.SetActive(false);
                
                currentForm = _characterForm;
                currentForm.gameObject.SetActive(true);
                
                outlinable.AddAllChildRenderersToRenderingList();

                statUI.transform.DOLocalMove(currentForm.statUIPos.localPosition, 0.2f);
                sizeUI.transform.DOLocalMove(currentForm.statSizePos.localPosition, 0.2f);
                intentionUI.transform.DOLocalMove(currentForm.statIntentionPos.localPosition, 0.2f);
            }
            else
            {
                Debug.LogError($"Form {_form.name} doesn't exist in {name} object");
            }
        }

        #endregion

        public IEnumerator OnKilled()
        {
            if (deathParticle)
            {
                var _particle = Instantiate(deathParticle);
                _particle.transform.position = transform.position;
            }

            //TODO: play animation
            yield return new WaitForSeconds(1f);
            
            Destroy(gameObject);
        }

        public IEnumerator PlayAnimation(string _anim)
        {
            if (!currentForm || !currentForm.skeletonAnimation || currentForm.skeletonAnimation.Skeleton.Data == null) yield break;
            
            var _animation = currentForm.skeletonAnimation.Skeleton.Data.FindAnimation(_anim);
            if (_animation != null)
            {
                var _duration = _animation.Duration;
                currentForm.skeletonAnimation.AnimationState.SetAnimation(0, _anim, false);

                yield return new WaitForSeconds(_duration);
                
                //Change back to idle after finished
                PlayIdleAnimation();
            }
        }

        public void PlayIdleAnimation()
        {
            PlayLoopAnimation(AnimationKey.IDLE);
        }
        
        public void PlayLoopAnimation(string _anim)
        {
            currentForm.skeletonAnimation.AnimationState.SetAnimation(0, _anim, true);
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