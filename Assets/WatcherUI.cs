using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefaultNamespace;
using AYellowpaper.SerializedCollections;
using DG.Tweening;

public class WatcherUI : MonoBehaviour
{
    [Tooltip("Reference Prefab")]
    [SerializeField] WatcherDetail _watcherDetailPrefab;
    [SerializeField] Canvas _mainCanvas;
    [SerializeField] CanvasGroup _canvasGroup;
    [SerializeField] Transform _content;
    [SerializeField] SerializedDictionary<string, WatcherDetail> _details;

    void Start()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.gameObject.SetActive(false);
    }

    public void AddDetail(Size size, string text)
    {
        WatcherDetail detailUI;
        string key = size.ToString();

        if (!_details.TryGetValue(key, out detailUI))
        {
            detailUI = Create();
            _details.Add(key, detailUI);
        }
        detailUI.SetDetail(size, text);

        detailUI.gameObject.SetActive(false);
        detailUI.gameObject.SetActive(true);
    }

    public void AddDetail(int size, string text)
    {
        WatcherDetail detailUI;
        string key = size.ToString();

        if (!_details.TryGetValue(key, out detailUI))
        {
            detailUI = Create();
            _details.Add(key, detailUI);
        }
        detailUI.SetDetail(size, text);

        detailUI.gameObject.SetActive(false);
        detailUI.gameObject.SetActive(true);
    }

    public void Show()
    {
        _canvasGroup.gameObject.SetActive(true);
        _canvasGroup.DOFade(1, 0.2f);
    }

    public void Hide()
    {
        _canvasGroup.DOFade(0, 0.1f).OnComplete(() => { _canvasGroup.gameObject.SetActive(false); });
    }

    public void Focus(Size size)
    {
        string key = size.ToString();
        foreach (KeyValuePair<string, WatcherDetail> kvp in _details)
        {
            if (kvp.Key == key) kvp.Value.Highlight();
            else kvp.Value.UnHighlight();
        }


    }

    public void Focus(int size)
    {
        string key = size.ToString();
        foreach (KeyValuePair<string, WatcherDetail> kvp in _details)
        {
            if (kvp.Key == key) kvp.Value.Highlight();
            else kvp.Value.UnHighlight();
        }
    }

    void Sorting()
    {

    }

    public WatcherDetail Create()
    {
        WatcherDetail detail = Instantiate(_watcherDetailPrefab);
        detail.transform.SetParent(_content);
        detail.transform.localScale = new Vector3(1,1,1);
        return detail;
    }

    //public void Check

}
