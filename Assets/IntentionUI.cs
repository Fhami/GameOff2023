using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefaultNamespace;
using DG.Tweening;
using AYellowpaper.SerializedCollections;

public class IntentionUI : MonoBehaviour
{
    [SerializeField] IntentionIcon _intention_prefab;
    [SerializeField] GameObject _content;
    [SerializeField] List<IntentionIcon> _use_intentions = new List<IntentionIcon>();
    [SerializeField] List<IntentionIcon> _pool_intentions = new List<IntentionIcon>();
    [SerializeField] SerializedDictionary<IntentData,int> _intentDatas;
    [SerializeField] IntentVFXDatabase _intentVfxDB;

    [SerializeField] IntentData intent01;
    [SerializeField] IntentData intent02;

    [SerializeField] private float showDuration = 0.1f;
    
    public IEnumerator SetIntention(List<IntentionDetail> intentDetails, bool withAnimation = true)
    {
        ClearIntention();
        foreach (var intentionDetail in intentDetails)
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

            icon.IntentionDetail = intentionDetail;

            if(_intentVfxDB._intentVFXs.TryGetValue(intentionDetail._intentData,out var refObject))
            {
                if (refObject)
                {
                    icon.SetIcon(refObject);
                }
                else
                {
                    icon.SetIcon(intentionDetail._intentData.icon);
                }
            }
            else
            {
                icon.SetIcon(intentionDetail._intentData.icon);
            }

            //if (_intentVfxDB._intentVFXs.TryGetValue(intentDetails[i]._intentData, out var obj)) 
            //{
            //    icon.SetVFXIcon(obj);
            //}
            //else
            //{
            //    icon.SetIcon(intentDetails[i]._intentData.icon);
            //}

            if (intentionDetail._value >= 0) icon.SetValue(intentionDetail._value);
            icon.SetMultiplier(intentionDetail._multiplier);
            icon.SetSizeEffect(intentionDetail._size);
            icon.gameObject.SetActive(true);

            if (withAnimation)
            {
                icon.transform.localScale = Vector3.zero;
                yield return icon.transform.DOScale(1, showDuration);
            }
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
    public enum ValueMod {None, Increase,Decrease}

    public IntentData _intentData;
    public int _value;
    public ValueMod _value_mod;
    public string _description;
    public int _multiplier;
    public ValueMod _multiplier_mod;
    public Size _size = Size.M;

    public IntentionDetail(IntentData intentData, int value)
    {
        this._intentData = intentData;
        this._value = value;
        this._size = Size.M;
    }

    public IntentionDetail(IntentData intentData, int value,int multiplier)
    {
        this._intentData = intentData;
        this._value = value;
        this._multiplier = multiplier;
        this._size = Size.M;
    }

    public IntentionDetail(IntentData intentData, int value, int multiplier, string description)
    {
        this._intentData = intentData;
        this._value = value;
        this._multiplier = multiplier;
        this._description = description;
        this._size = Size.M;
    }

    public IntentionDetail SetSize(Size size)
    {
        this._size = size;
        return this;
    }

    public IntentionDetail SetValueMod(ValueMod mod)
    {
        this._value_mod = mod;
        return this;
    }

    public IntentionDetail SetMultiplierMod(ValueMod mod)
    {
        this._multiplier_mod = mod;
        return this;
    }

}