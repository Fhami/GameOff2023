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
    public class Character : MonoBehaviour, ICardTarget
    {
        public Transform FrontPos => currentForm ? currentForm.frontPos : transform;
        public RuntimeCharacter runtimeCharacter;
        public CardController cardController;

        [SerializeField] private SerializedDictionary<FormData, CharacterForm> characterForms;
        public CharacterForm currentForm;

        [SerializeField] private Outlinable outlinable;
        [Foldout("UI"), SerializeField] private StatsUI statUI;
        [Foldout("UI"), SerializeField] private SizeUI sizeUI;
        [Foldout("UI"), SerializeField] private IntentionUI intentionUI;

        [SerializeField]
        private SerializedDictionary<ParticleKey, ParticleSystem> particles =
            new SerializedDictionary<ParticleKey, ParticleSystem>();

        public void Init(RuntimeCharacter _runtimeCharacter)
        {
            runtimeCharacter = _runtimeCharacter;
            runtimeCharacter.Character = this;
            
            //Hide all form first
            foreach (var _form in characterForms.Values)
            {
                _form.gameObject.SetActive(false);
            }

            InitSizeUI();
            
            //Update visual
            UpdateHpVisual(0, runtimeCharacter.properties.Get<int>(PropertyKey.HEALTH));
            UpdateSizeVisual(0, runtimeCharacter.properties.Get<int>(PropertyKey.SIZE).Value);
            UpdateShield(0, runtimeCharacter.properties.Get<int>(PropertyKey.SHIELD));
            UpdateFormVisual(runtimeCharacter.GetCurrentForm());

            runtimeCharacter.properties.Get<int>(PropertyKey.HEALTH).OnChanged += UpdateHpVisual;
            runtimeCharacter.properties.Get<int>(PropertyKey.SHIELD).OnChanged += UpdateShield;
        }

        private void InitSizeUI()
        {
            var _formCount = runtimeCharacter.characterData.forms.Count;
            var _smallForm = runtimeCharacter.characterData.forms[0];
            var _bigForm = runtimeCharacter.characterData.forms[_formCount - 1];
            var _small = _formCount > 1 ? _smallForm.sizeMax : -1;
            var _big = _formCount > 1 ? _bigForm.sizeMin : -1;
            var _smallDeath = _smallForm.sizeMin == 0 ? 0 : -1;
            var _bigDeath = runtimeCharacter.characterData.deathOnMax ? _bigForm.sizeMax : -1;

            sizeUI.InitSizeUI(runtimeCharacter.properties.Get<int>(PropertyKey.SIZE).Value, _small,
                _big, _smallDeath, _bigDeath);
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
                PlayParticle(ParticleKey.DAMAGED);

                StartCoroutine(PlayAnimation(AnimationKey.HIT));
            }
            else
            {
                PlayParticle(ParticleKey.HEAL);
            }
        }

        public void UpdateShield(int _oldValue, Property<int> _value)
        {
            statUI.SetShield(_oldValue,_value.Value);
        }

        public void UpdateSizeVisual(int _oldValue, int _size)
        {
            var _sizeEffect = _oldValue > _size ? SizeEffectType.Increase : SizeEffectType.Decrease;
            sizeUI.SetSize(_size, _sizeEffect);
        }

        public IEnumerator UpdateIntention(RuntimeCard _runtimeCard)
        {
            if (_runtimeCard == null) yield break;
            
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
            Debug.Log($"{name} intent: {_runtimeCard.cardData.name}");
            
            yield return intentionUI.SetIntention(_intentDetails);
        }

        public void UpdateFormVisual(FormData _form)
        {
            if (characterForms.TryGetValue(_form, out var _characterForm))
            {
                if (currentForm == _characterForm) return;

                PlayParticle(ParticleKey.CHANGED_FORM);
                
                if (currentForm)
                    currentForm.gameObject.SetActive(false);
                
                currentForm = _characterForm;
                currentForm.gameObject.SetActive(true);
                
                switch (_form.size)
                {
                    case Size.S:
                        break;
                    case Size.M:
                        break;
                    case Size.L:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
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

        public void UpdateBuffsAndDebuffsVisual()
        {
            foreach (var _buffs in runtimeCharacter.GetActiveBuffsAndDebuffs())
            {

                if (Database.buffData.TryGetValue(_buffs.Key, out var _buffData))
                {
                    statUI.SetBuff(_buffData, _buffs.Value);
                }
            }
        }

        #endregion
        
        /// <summary>
        /// Using target as direction to move
        /// </summary>
        public IEnumerator PlayAttackFeedback(Transform _target)
        {
            var _origin = transform.position;
            transform.DOMove(_target.position, 0.2f).SetEase(Ease.Flash);

            yield return PlayAnimation(AnimationKey.ATTACK);
            PlayParticle(ParticleKey.ATTACK);
            
            yield return transform.DOMove(_origin, 0.2f).WaitForCompletion();
        }
        
        public IEnumerator OnKilled()
        {
            Debug.Log($"{name} is death");
            PlayParticle(ParticleKey.DEATH);
            
            yield return PlayAnimation(AnimationKey.HIT);
            yield return new WaitForSeconds(0.5f);
            
            Destroy(gameObject);
        }

        #region Animation

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

        #endregion

        public void PlayParticle(ParticleKey _key)
        {
            if (!particles.TryGetValue(_key, out var _prefab)) return;
            if (!_prefab) return;
            
            var _particle = Instantiate(_prefab);
            _particle.transform.position = transform.position;
            _particle.Play();
        }
        
        /// <summary>
        /// Get current enemy's intention
        /// </summary>
        public RuntimeCard GetIntention()
        {
            FormData _form = runtimeCharacter.GetCurrentForm();
            Property<int> _cardIndex = runtimeCharacter.properties.Get<int>(PropertyKey.ENEMY_ATTACK_PATTERN_CARD_INDEX);
            if (_cardIndex.Value > _form.attackPattern.Count - 1)
            {
                return null;
            }
            CardData _cardData = _form.attackPattern[_cardIndex.Value];
            RuntimeCard _card = CardFactory.Create(_cardData);

            return _card;
        }
    }
}