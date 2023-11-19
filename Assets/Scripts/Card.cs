using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace
{
    // TODO: This can be attached to card prefab
    public class Card : MonoBehaviour
    {
        public UnityEvent<Card> OnDrag;
        public UnityEvent<ICardTarget> OnEnterTarget;
        public UnityEvent<ICardTarget> OnExistTarget;
        public UnityEvent<ICardTarget> OnDropped;
        
        public RuntimeCard runtimeCard;

        [SerializeField] private TextMeshPro nameTxt;
        [SerializeField] private TextMeshPro effectTxt;
        
        [SerializeField] private MMF_Player enterTargetPlayer;
        [SerializeField] private MMF_Player existTargetPlayer;

        [SerializeField] private LayerMask targetMask;
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
        }

        /// <summary>
        /// Call after init or when player size changed to update card's effect value
        /// </summary>
        /// <param name="_character"></param>
        public void UpdateCard(RuntimeCharacter _character)
        {
            StringBuilder _builder = new StringBuilder();
            foreach (var _effect in runtimeCard.cardData.effects)
            {
                _builder.AppendLine(_effect.GetDescriptionTextWithModifiers(runtimeCard, _character, _character, null, null));
            }
            
            effectTxt.SetText(_builder.ToString());
        }

        public bool ValidateTarget(ICardTarget _target)
        {
            // if ((runtimeCard.cardData.cardDragTarget & CardDragTarget.BACKGROUND) != 0)
            // {
            //     //play on background
            // }
            
            return validTargets.Contains(_target);
        }
        
        public void ClearCallBack()
        {
            OnDrag.RemoveAllListeners();
            OnDropped.RemoveAllListeners();
            OnEnterTarget.RemoveAllListeners();
            OnExistTarget.RemoveAllListeners();
        }

        private void OnMouseDown()
        {
            validTargets = GetValidTargets(runtimeCard);
        }

        private void OnMouseDrag()
        {
            HighlightTargets(true);

            var _hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, targetMask);

            if (_hit)
            {
                //Debug.Log($"hit {_hit.transform.gameObject.name} {Camera.main.ScreenToWorldPoint(Input.mousePosition)}");
                var _target = _hit.transform.gameObject.GetComponent<ICardTarget>();
                SetCurrentTarget(_target);
                
            }
            else
            {
                SetCurrentTarget(null);
                
            }

            OnDrag?.Invoke(this);
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
                        Debug.Log($"{_tag} {_target.name}");
                        _results.Add(_validTarget);
                    }
                }
            }

            return _results;
        }
    }
}