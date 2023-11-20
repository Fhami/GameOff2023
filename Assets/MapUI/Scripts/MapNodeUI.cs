using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;

public class MapNodeUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] NodeInfo nodeInfo;
    [SerializeField] EncounterData encounterData;
    [SerializeField] RectTransform _rect; 
    [SerializeField] Image _parent_img;
    [SerializeField] Image _ground_img;
    [SerializeField] Image _icon_img;
    [SerializeField] TextMeshProUGUI _name_txt;
    [SerializeField] List<MapNodeUI> _prevNodes = new List<MapNodeUI>();
    [SerializeField] List<MapNodeUI> _nextNodes = new List<MapNodeUI>();
    [SerializeField] List<MapLineUI> _lines = new List<MapLineUI>();
    [SerializeField] Animator _animator;
    [SerializeField] ParticleSystem _unlockParticle;

    [SerializeField] Color _enableDimColor;
    [SerializeField] Color _disableDimColor;
    [SerializeField] Color _disableEnemyColor;//464646

    public Action<MapNodeUI> onClick;
    public bool isLock = false;
    public int row = 0;

    public Image Parent_img { get => _parent_img; set => _parent_img = value; }
    public Image Ground_img { get => _ground_img; set => _ground_img = value; }
    public Image Icon_img { get => _icon_img; set => _icon_img = value; }
    public TextMeshProUGUI Name_txt { get => _name_txt; set => _name_txt = value; }
    public RectTransform Rect { get => _rect; set => _rect = value; }
    public List<MapNodeUI> PrevNodes { get => _prevNodes; set => _prevNodes = value; }
    public List<MapNodeUI> NextNodes { get => _nextNodes; set => _nextNodes = value; }
    public List<MapLineUI> Lines { get => _lines; set => _lines = value; }
    public NodeInfo NodeInfo { get => nodeInfo; set => nodeInfo = value; }
    public EncounterData EncounterData { get => encounterData; set => encounterData = value; }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke(this);
    }

    public void SetLock(bool val)
    {
        isLock = val;
    }

    public void Unlock()
    {
        SetLock(false);
        _animator.SetTrigger("unlock");
       // _unlockParticle.gameObject.SetActive(true);
    }

    public void Disable()
    {
        SetLock(true);
        _ground_img.color = _disableDimColor;
        _icon_img.color = _disableEnemyColor;
        //_animator.SetTrigger("disable");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isLock) return;
        _animator.SetTrigger("enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isLock) return;
        _animator.SetTrigger("exit");
    }

    public void SetImage(Sprite sprite)
    {
        _icon_img.sprite = sprite;
    }
}
