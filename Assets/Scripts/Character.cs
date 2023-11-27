using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using EPOOutline;
using NaughtyAttributes;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public enum DeathCondition
    {
        SMALL,
        BIG,
        HP
    }
    
    public class Character : MonoBehaviour, ICardTarget
    {
        public Transform FrontPos => currentForm ? currentForm.frontPos : transform;
        public RuntimeCharacter runtimeCharacter;
        public CardController cardController;
        public Transform visual;
        [SerializeField] private SerializedDictionary<FormData, CharacterForm> characterForms;
        public CharacterForm currentForm;

        [SerializeField] private Outlinable outlinable;
        [Foldout("UI"), SerializeField] public StatsUI statUI;
        [Foldout("UI"), SerializeField] public SizeUI sizeUI;
        [Foldout("UI"), SerializeField] public IntentionUI intentionUI;
        [Foldout("UI"), SerializeField] public ActiveSkillUI activeSkillUI;
        [Foldout("UI"), SerializeField] public WatcherUI watcherUI;

        [SerializeField]
        private SerializedDictionary<FXKey, ParticleSystem> particles =
            new SerializedDictionary<FXKey, ParticleSystem>();

        [SerializeField]
        private SerializedDictionary<FXKey, AudioClip> audioClips = new SerializedDictionary<FXKey, AudioClip>();

        public void Init(RuntimeCharacter _runtimeCharacter)
        {
            runtimeCharacter = _runtimeCharacter;
            runtimeCharacter.Character = this;

            statUI.RuntimeCharacter = runtimeCharacter;
            
            //Hide all form first
            foreach (var _form in characterForms.Values)
            {
                _form.gameObject.SetActive(false);
            }
            
            var form = runtimeCharacter.GetCurrentForm();
            foreach (var _passive in runtimeCharacter.passiveSlots[form])
            {
                if (_passive == null) continue;
                
                if (_passive.passiveData.triggerGameEvent != GameEvent.NONE)
                {
                    runtimeCharacter.DisablePassive(_passive);
                }
            }

            InitSizeUI();
            InitActiveSkillUI();
            InitWatcherUI();
            
            //Update visual
            UpdateHpVisual(0, runtimeCharacter.properties.Get<int>(PropertyKey.HEALTH));
            UpdateSizeVisual(0, runtimeCharacter.properties.Get<int>(PropertyKey.SIZE).Value);
            UpdateShield(0, runtimeCharacter.properties.Get<int>(PropertyKey.SHIELD));
            UpdateFormVisual(null, runtimeCharacter.GetCurrentForm());
            UpdatePassiveIcon(null, runtimeCharacter.GetCurrentForm());

            runtimeCharacter.properties.Get<int>(PropertyKey.HEALTH).OnChanged += UpdateHpVisual;
            runtimeCharacter.properties.Get<int>(PropertyKey.SHIELD).OnChanged += UpdateShield;

            foreach (PropertyKey buffPropertyKey in Database.buffData.Keys)
            {
                if (buffPropertyKey == PropertyKey.NONE) continue;
                runtimeCharacter.properties.Get<int>(buffPropertyKey).OnChanged += UpdateBuffVisual;
            }
            
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


            var _sizeSetting = new SizeSetting(_small, (_smallForm.sizeMax + _bigForm.sizeMin) / 2, _big,
                runtimeCharacter.characterData.minSize, runtimeCharacter.characterData.maxSize, _smallDeath, _bigDeath);

            foreach (var _form in runtimeCharacter.characterData.forms)
            {
                if (_form.activeSkill)
                {
                    _sizeSetting.skills.Add(_form.activeSkillSize);
                }
            }
            
            sizeUI.InitSizeUI(runtimeCharacter.characterData.startSize, _sizeSetting);
        }

        private void InitActiveSkillUI()
        {
            for (var _index = 0; _index < runtimeCharacter.characterData.forms.Count; _index++)
            {
                var _formData = runtimeCharacter.characterData.forms[_index];
                if (_formData.activeSkill)
                {
                    activeSkillUI.gameObject.SetActive(true);
                    activeSkillUI.SetSkill(_index, new ActiveSkillDetail(_formData.activeSkill, _formData.activeSkillSize, _formData.size),
                        () =>
                        {
                            //TODO: Play activeSkill from BattleManager
                        });
                    activeSkillUI.EnableSkill(_formData.activeSkill, true);
                }
                else
                {
                    activeSkillUI.RemoveSkill(_index);
                }
            }
        }

        private void InitWatcherUI()
        {
            foreach (var _passiveSlot in runtimeCharacter.passiveSlots)
            {
                foreach (var _runtimePassive in _passiveSlot.Value)
                {
                    if (_runtimePassive == null) continue;
                    var _sb = new StringBuilder();
                    
                    _sb.AppendLine(_runtimePassive.passiveData.GetDescription());

                    if (_runtimePassive.passiveData.triggerConditions.Count > 0)
                    {
                        watcherUI.AddDetail((PropertyCondition)_runtimePassive.passiveData.triggerConditions[0], _sb.ToString());
                    }
                    else
                    {
                        watcherUI.AddDetail(_passiveSlot.Key.size, _sb.ToString());
                    }
                }
            }
        }

        public IEnumerator UpdateSize(int _oldValue, int _size)
        {
            UpdateSizeVisual(_oldValue, _size);
            foreach (var _formData in runtimeCharacter.characterData.forms)
            {
                if (!_formData.activeSkill) continue;
                
                if (_formData.activeSkillSize == _size)
                {
                    Debug.Log($"Get Skill card {_formData.activeSkill.name}");
                    var _skillCard = CardFactory.Create(_formData.activeSkill);
                    yield return BattleManager.current.CreateCardAndAddItToHand(_skillCard);
                }
            }
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

        public void UpdateBuffVisual(int _oldValue, Property<int> _value)
        {
            if (Database.buffData.TryGetValue(_value.Key, out var _buffData))
            {
                statUI.SetBuff(_buffData, _value.GetValueWithModifiers(runtimeCharacter));
            }
            
            BattleManager.current.CardController.UpdateCards();
        }

        public void UpdateHpVisual(int _oldValue, Property<int> _value)
        {
            statUI.SetHp(_oldValue, _value.Value, runtimeCharacter.properties.Get<int>(PropertyKey.MAX_HEALTH).Value);

            if (_oldValue > _value.Value)
            {
                //Play animation
                PlayParticle(FXKey.DAMAGED);
                StartCoroutine(PlayAnimation(AnimationKey.HIT));
            }
            else
            {
                PlayParticle(FXKey.HEAL);
            }
        }

        public void UpdateShield(int _oldValue, Property<int> _value)
        {
            statUI.SetShield(_oldValue,_value.Value);
            if (_oldValue > 0)
            {
                PlayParticle(FXKey.BLOCKED);
            }

            if (_oldValue < _value.Value)
            {
                PlayAudio(FXKey.GAIN_SHIELD);
            }
        }

        public void UpdateSizeVisual(int _oldValue, int _size)
        {
            //var _sizeEffect = _oldValue > _size ? SizeEffectType.Increase : SizeEffectType.Decrease;
            sizeUI.GoToSize(_oldValue, _size);

            var _defaultSize = runtimeCharacter.characterData.startSize;
            var _diff = _size - _defaultSize;
            float _mult = 1f + (_diff * 0.1f);
            
            visual.DOScale(Vector3.one * _mult, 0.1f);
            sizeUI.transform.DOLocalMoveY(_mult * 0.1f, 0.1f);

            PlayParticle(_oldValue > _size ? FXKey.SIZE_DOWN : FXKey.SIZE_UP);
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

        public void UpdateFormVisual(FormData _prevForm, FormData _form)
        {
            if (characterForms.TryGetValue(_form, out var _characterForm))
            {
                if (currentForm == _characterForm) return;

                if (_prevForm)
                {
                    PlayParticle(_prevForm.sizeMax < _form.sizeMax ? FXKey.FORM_UP : FXKey.FORM_DOWN);
                }

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

        public void UpdatePassiveIcon(FormData _prevForm, FormData _currentForm)
        {
            if (_prevForm)
            {
                foreach (var _passiveData in _prevForm.passives)
                {
                    statUI.SetBuff(_passiveData.buffData, -1);
                }
            }

            foreach (var _passiveData in _currentForm.passives)
            {
                if (_passiveData.triggerGameEvent == GameEvent.NONE)
                {
                    statUI.SetBuff(_passiveData.buffData, _passiveData.value);
                }
            }
            
        }

        public void UpdateBuffsAndDebuffsVisual()
        {
            foreach (var _buffs in runtimeCharacter.GetActiveBuffsAndDebuffs())
            {
                if (Database.buffData.TryGetValue(_buffs.Key, out var _buffData))
                {
                    statUI.SetBuff(_buffData, _buffs.GetValueWithModifiers(runtimeCharacter));
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
            //transform.DOMove(_target.position, 0.2f).SetEase(Ease.Flash);

            yield return PlayAnimation(AnimationKey.ATTACK);
            PlayParticle(FXKey.ATTACK);
            
            //yield return transform.DOMove(_origin, 0.2f).WaitForCompletion();
        }
        
        public IEnumerator OnKilled(FXKey _condition)
        {
            Debug.Log($"{name} is death");
            PlayParticle(FXKey.DEATH);
            UnSubScribeProperties();
            
            yield return PlayAnimation(AnimationKey.HIT);
            yield return new WaitForSeconds(0.5f);

            Destroy(gameObject);
        }

        private void UnSubScribeProperties()
        {
            runtimeCharacter.properties.Get<int>(PropertyKey.HEALTH).OnChanged -= UpdateHpVisual;
            runtimeCharacter.properties.Get<int>(PropertyKey.SHIELD).OnChanged -= UpdateShield;

            foreach (PropertyKey buffPropertyKey in Database.buffData.Keys)
            {
                if (buffPropertyKey == PropertyKey.NONE) continue;
                runtimeCharacter.properties.Get<int>(buffPropertyKey).OnChanged -= UpdateBuffVisual;
            }
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

        public void PlayParticle(FXKey _key)
        {
            if (!particles.TryGetValue(_key, out var _prefab)) return;
            if (!_prefab) return;
            
            var _particle = Instantiate(_prefab);
            _particle.transform.position = transform.position;
            _particle.Play();
            
            PlayAudio(_key);
        }

        public void PlayAudio(FXKey _key)
        {
            if (!audioClips.TryGetValue(_key, out var _clip)) return;
            if (!_clip) return;
            
            SoundManager.Instance.PlaySFX(_clip);
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

        private void OnMouseEnter()
        {
            watcherUI.Show();
        }

        private void OnMouseExit()
        {
            watcherUI.Hide();
        }
    }
}