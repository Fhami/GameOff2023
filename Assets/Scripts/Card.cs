using System;
using System.Collections.Generic;
using System.Text;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    // TODO: This can be attached to card prefab
    public class Card : MonoBehaviour
    {
        public RuntimeCard runtimeCard;

        [SerializeField] private TextMeshPro nameTxt;
        [SerializeField] private TextMeshPro effectTxt;
        [SerializeField] private DragableObject dragableObject;
        
        /// <summary>
        /// Init card data, need to call UpdateCard afterward to update effects text
        /// </summary>
        /// <param name="_card"></param>
        public void InitCard(RuntimeCard _card)
        {
            runtimeCard = _card;
            nameTxt.SetText(_card.cardData.name);
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
                _builder.AppendLine(_effect.GetDescriptionText(runtimeCard, _character));
            }
            
            effectTxt.SetText(_builder.ToString());
        }

        private void OnMouseDrag()
        {
            //var _targets
        }

        public List<Character> GetValidTarget()
        {
            var _results = new List<Character>();
            var _targetTags = runtimeCard.cardData.cardDragTarget;
            foreach (var _tag in Enum.GetValues(_targetTags.GetType()))
            {
                var _targets = GameObject.FindGameObjectsWithTag(_tag.ToString());
                
                foreach (var _target in _targets)
                {
                    if (_target.TryGetComponent<Character>(out var _character))
                    {
                        _results.Add(_character);
                    }
                }
            }

            return _results;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            //TODO: Check correct target
        }
    }
}