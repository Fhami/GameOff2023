using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefaultNamespace;

public class IntentionUI : MonoBehaviour
{
    [SerializeField] IntentionIcon _intention_prefab;
    [SerializeField] GameObject _content;
    [SerializeField] List<IntentionIcon> _use_intentions = new List<IntentionIcon>();
    [SerializeField] List<IntentionIcon> _pool_intentions = new List<IntentionIcon>();

    [SerializeField] IntentData intent01;
    [SerializeField] IntentData intent02;

    public void SetIntention(List<IntentionDetail> intentDetails)
    {
        ClearIntention();
        for (int i = 0; i < intentDetails.Count; i++)
        {
            IntentionIcon icon;
            if (_pool_intentions.Count > 0)
            {
                icon = _pool_intentions[0];
                _use_intentions.Add(icon);
                _pool_intentions.RemoveAt(0);
            }
            else
            {
                icon = CreateIntentionIcon();
                _use_intentions.Add(icon);
            }
            icon.SetIcon(intentDetails[i]._intentData.icon);
            if(intentDetails[i]._value >= 0) icon.SetValue(intentDetails[i]._value);
            icon.gameObject.SetActive(true);
        }
    }


    protected IntentionIcon CreateIntentionIcon()
    {
        var icon =  Instantiate<IntentionIcon>(_intention_prefab);
        icon.transform.SetParent(_content.transform);
        icon.transform.localScale = new Vector3(1, 1, 1);
        return icon;
    }


    protected void ClearIntention()
    {
        foreach(var intent in _use_intentions)
        {
            intent.gameObject.SetActive(false);
            _pool_intentions.Add(intent);
        }
        _use_intentions.Clear();
    }

    #region TestFunction



    public void Btn_Add01()
    {
        List<IntentionDetail> intentDetails = new List<IntentionDetail>() {
            new IntentionDetail(intent01,2,1,""),
            new IntentionDetail(intent02, -1,1, "")
        };
        SetIntention(intentDetails);
    }

    public void Btn_Add02()
    {
        List<IntentionDetail> intentDetails = new List<IntentionDetail>();
        intentDetails.Add(new IntentionDetail(intent01, 2, 1, ""));
        intentDetails.Add(new IntentionDetail(intent02, -1, 1, ""));
        intentDetails.Add(new IntentionDetail(intent02, 4, 1, ""));
        intentDetails.Add(new IntentionDetail(intent02, 4, 1, ""));
        SetIntention(intentDetails);
    }
    #endregion
}

public class IntentionDetail
{
    public IntentData _intentData;
    public int _value;
    public string _description;
    public int _multiplier;

    public IntentionDetail(IntentData intentData, int value)
    {
        this._intentData = intentData;
        this._value = value;
    }

    public IntentionDetail(IntentData intentData, int value,int multiplier)
    {
        this._intentData = intentData;
        this._value = value;
        this._multiplier = multiplier;
    }

    public IntentionDetail(IntentData intentData, int value, int multiplier, string description)
    {
        this._intentData = intentData;
        this._value = value;
        this._description = description;
    }
}