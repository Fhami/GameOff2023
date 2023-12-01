using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AYellowpaper.SerializedCollections;
using Coffee.UIExtensions;
using DG.Tweening;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class Card : MonoBehaviour
    {
        [Foldout("Event")] public UnityEvent<Card> OnDrag;
        [Foldout("Event")] public UnityEvent<ICardTarget> OnEnterTarget;
        [Foldout("Event")] public UnityEvent<ICardTarget> OnExistTarget;
        [Foldout("Event")] public UnityEvent<ICardTarget> OnDropped;
        
        public RuntimeCard runtimeCard { get; private set; }

        [SerializeField] private TextMeshPro nameTxt;
        [SerializeField] private TextMeshPro effectTxt;
        
        [Header("Feedback")]
        [SerializeField] private MMF_Player enterTargetPlayer;
        [SerializeField] private MMF_Player existTargetPlayer;

        [Header("VFX")] 
        [SerializeField] private UIParticle exhaustCardParticle;
        [SerializeField] private UIParticle destroyCardParticle;

        [Header("Visual")] 
        [SerializeField] SpriteRenderer cardArt;
        [SerializeField] private DraggableLine draggableLine;
        [SerializeField] private SerializedDictionary<CardType, GameObject> visualDict;
        [SerializeField] private SerializedDictionary<Size, GameObject> borderDict;

        [SerializeField] private LayerMask targetMask;
        public Collider2D Collider;
        private List<ICardTarget> validTargets = new List<ICardTarget>();
        private ICardTarget currentTarget;

        public bool Unplayable
        {
            get
            {
                return runtimeCard.cardData.effects.Exists(x => x is UnplayableEffect);
            }
        }
        
        /// <summary>
        /// Init card data, need to call UpdateCard afterward to update effects text
        /// </summary>
        /// <param name="_runtimeCard"></param>
        public void InitCard(RuntimeCard _runtimeCard)
        {
            runtimeCard = _runtimeCard;
            runtimeCard.Card = this;
            nameTxt.SetText(_runtimeCard.cardData.name);

            foreach (var _obj in visualDict.Values)
            {
                _obj.SetActive(false);
            }

            foreach (var _obj in borderDict.Values)
            {
                _obj.SetActive(false);
            }
            
            if (visualDict.TryGetValue(runtimeCard.cardData.cardType, out var _value))
            {
                _value.SetActive(true);
            }

            if (borderDict.TryGetValue(runtimeCard.cardData.cardSize, out var _size))
            {
                _size.SetActive(true);
            }

            cardArt.sprite = _runtimeCard.cardData.cardImage;
        }

        /// <summary>
        /// Call after init or when player size changed to update card's effect value
        /// </summary>
        /// <param name="_character"></param>
        public void UpdateCard(RuntimeCharacter _character)
        {
            effectTxt.SetText(RuntimeCard.GetCardDescriptionWithModifiers(_character, runtimeCard));
        }

        public IEnumerator DestroyCard()
        {
            yield return transform.DOScale(Vector3.zero, 0.3f).WaitForCompletion();;//TODO: Play vfx here

            Destroy(gameObject);
        }

        public IEnumerator ExhaustCard()
        {
            yield return transform.DOScale(Vector3.zero, 0.3f).WaitForCompletion();
        }

        public bool ValidateTarget(ICardTarget _target)
        {
            return validTargets.Contains(_target);
        }
        
        public void ClearCallBack()
        {
            OnDrag.RemoveAllListeners();
            OnDropped.RemoveAllListeners();
            OnEnterTarget.RemoveAllListeners();
            OnExistTarget.RemoveAllListeners();
        }

        #region Dragging

        private void OnMouseDown()
        {
            validTargets = GetValidTargets(runtimeCard);
            
        }

        private void OnMouseDrag()
        {
            if (Unplayable || !BattleManager.current.canPlayCard)
            {
                draggableLine.Cancel();
                draggableLine.enabled = false;
                return;
            }

            draggableLine.enabled = true;

            HighlightTargets(true);

            var _hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, targetMask);

            if (_hit)
            {
                //Debug.Log($"hit {_hit.transform.gameObject.name} {Camera.main.ScreenToWorldPoint(Input.mousePosition)}");
                var _target = _hit.transform.gameObject.GetComponent<ICardTarget>();
                SetCurrentTarget(_target);
                if (_target != null)
                {
                    PreviewStat(_target, true);
                }
                else
                {
                    PreviewStat(null, false);
                }
            }
            else
            {
                SetCurrentTarget(null);
                PreviewStat(null, false);
            }

            OnDrag?.Invoke(this);
        }

        private void PreviewStat(ICardTarget _newTarget, bool _value)
        {
            if (_value)
            {
                //if (currentTarget == _newTarget) return;
                if (!ValidateTarget(_newTarget))
                {
                    PreviewStat(_newTarget, false);
                    return;
                }
                if (!BattleManager.current.canPlayCard) return;

                foreach (var _effect in runtimeCard.cardData.effects)
                {
                    _effect.PreviewEffect(runtimeCard, BattleManager.current.runtimePlayer,
                        BattleManager.current.runtimePlayer, currentTarget.GameObject.GetComponent<Character>()?.runtimeCharacter,
                        BattleManager.current.runtimeEnemies);
                }
            }
            else
            {
                //if (currentTarget == _newTarget) return;
                
                foreach (var _enemy in BattleManager.current.enemies)
                {
                    _enemy.statUI.CancelPreviewHp();
                    _enemy.statUI.CancelPreviewShield();
                }
            
                BattleManager.current.player.statUI.CancelPreviewHp();
                BattleManager.current.player.statUI.CancelPreviewShield();
            }
        }

        private void SetCurrentTarget(ICardTarget _newTarget)
        {
            currentTarget?.HighlightSelected(false);

            currentTarget = _newTarget;

            if (currentTarget != null)
            {
                currentTarget.HighlightSelected(true);

                enterTargetPlayer.PlayFeedbacks();
            }
            else
            {
                existTargetPlayer.PlayFeedbacks();
            }
        }
        
        private void OnMouseUp()
        {
            HighlightTargets(false);
            
            existTargetPlayer.PlayFeedbacks();
            
            if (ValidateTarget(currentTarget))
            {
                OnDropped?.Invoke(currentTarget);
            }
        }

        private void HighlightTargets(bool _value)
        {
            foreach (var _validTarget in validTargets)
            {
                if (_validTarget.GameObject)
                {
                    //Highlight target
                    _validTarget.Highlight(_value);
                }
            }
        }

        #endregion
        
        public static List<ICardTarget> GetValidTargets(RuntimeCard _runtimeCard)
        {
            var _results = new List<ICardTarget>();
            var _targetTags = _runtimeCard.cardData.cardDragTarget;
            foreach (CardDragTarget _tag in Enum.GetValues(typeof(CardDragTarget)))
            {
                if ((_targetTags & _tag) == 0) continue;
                if (_tag.ToString() == "NONE") continue;

                var _targets = GameObject.FindGameObjectsWithTag(_tag.ToString());

                foreach (var _target in _targets)
                {
                    if (_target.TryGetComponent<ICardTarget>(out var _validTarget))
                    {
                        //Debug.Log($"{_tag} {_target.name}");
                        _results.Add(_validTarget);
                    }
                }
            }

            return _results;
        }
    }
}