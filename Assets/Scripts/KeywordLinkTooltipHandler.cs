using System;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Text;
using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using UnityEngine.EventSystems;

namespace DefaultNamespace
{
    public class KeywordLinkTooltipHandler : MonoBehaviour
    {
        [Expandable]
        [SerializeField] private List<KeywordData> keywords;
        [SerializeField] private GameObject keywordCanvasPrefab;
        
        private Camera _mainCamera;
        private PointerEventData _pointerEventData;
        private EventSystem _eventSystem;
        private readonly List<RaycastResult> _results = new();

        private string _currentKeyword;
        private GameObject _currentKeywordCanvas;
        
        private void OnEnable()
        {
            _mainCamera = Camera.main;
            _eventSystem = FindFirstObjectByType<EventSystem>();
        }

        private void Update()
        {
            // NOTE: I'm not sure if we plan to have tooltip when mouse overing over keywords. If we want that, then we can use something like this.
            RaycastForTextMeshProUGUI();
        }

        private void LateUpdate()
        {
            if (_currentKeywordCanvas != null)
            {
                var position = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                position.z = -9;
                _currentKeywordCanvas.transform.position = position;
            }
        }

        private void RaycastForTextMeshProUGUI()
        {
            if (_mainCamera != null && _eventSystem != null)
            {
                // Clear previous raycast results
                _results.Clear();
                
                // Set up a new pointer event
                _pointerEventData = new PointerEventData(_eventSystem)
                {
                    position = Input.mousePosition
                };

                _eventSystem.RaycastAll(_pointerEventData, _results);

                string keyword = "";
                
                // Check for a hit on a TextMeshProUGUI component
                foreach (RaycastResult result in _results)
                {
                    if (result.gameObject.TryGetComponent(out TextMeshProUGUI text))
                    {
                        int link = TMP_TextUtilities.FindIntersectingLink(text, _pointerEventData.position, _mainCamera);

                        if (link != -1)
                        {
                            // You can use keyword as a link in the text and we can use the link to open a tool tip
                            // Example: <link="shield"><color=lightblue>Shield</color></link>
                            TMP_LinkInfo linkInfo = text.textInfo.linkInfo[link];
                            keyword = linkInfo.GetLinkText();
                            break;
                        }
                    }
                }

                UpdateCurrentKeyword(keyword);
            }
        }

        private void UpdateCurrentKeyword(string keyword)
        {
            keyword = keyword.ToLower();
            
            if (string.IsNullOrEmpty(keyword))
            {
                ClearCurrentKeyword();
            }
            else
            {
                OnGotKeyword(keyword);
            }
        }

        private void ClearCurrentKeyword()
        {
            _currentKeyword = "";
            if (_currentKeywordCanvas != null)
            {
                Destroy(_currentKeywordCanvas);
            }
        }
        
        private void OnGotKeyword(string keyword)
        {
            if (_currentKeyword == keyword)
            {
                return;
            }
            
            if (_currentKeywordCanvas != null)
            {
                ClearCurrentKeyword();
            }

            if (TryGetKeywordData(keyword, out KeywordData keywordData))
            {
                _currentKeyword = keyword;
                _currentKeywordCanvas = Instantiate(keywordCanvasPrefab);
                
                TextMeshProUGUI text = _currentKeywordCanvas.transform.GetComponentInChildren<TextMeshProUGUI>();

                StringBuilder sb = new();
                sb.AppendLine(keywordData.header);
                sb.AppendLine(keywordData.description);
                text.SetText(sb.ToString());
            }
        }

        private bool TryGetKeywordData(string keyword, out KeywordData keywordData)
        { 
            keywordData = keywords.Find(x => x.name == keyword);
            return keywordData != null;
        }
    }
}
