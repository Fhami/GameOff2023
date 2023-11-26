using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsUITester : MonoBehaviour
{
    [SerializeField] StatsUI statsUI;
    [SerializeField] int _maxHp = 100;
    [SerializeField] int _currentHp = 100;
    [SerializeField] int _currentShield = 0;

    #region Test Function

    public void Btn_AddHp(TMP_InputField value)
    {
       int v = _currentHp + int.Parse(value.text);
        if (v > 100) v = 100;
        if (v < 0) v = 0;
        statsUI.SetHp(_currentHp, v, _maxHp, 0.2f);
        _currentHp = v;
    }

    public void Btn_AddShield(TMP_InputField value)
    {
        var v = _currentShield + int.Parse(value.text);
        if (v < 0) v = 0;
        statsUI.SetShield(_currentShield, v);
        _currentShield = v;
    }

    public void Btn_AddBuff(BuffData buffData)
    {
        statsUI.SetBuff(buffData, 1);
    }

    public void Btn_RemoveBuff(BuffData buffData)
    {
        statsUI.SetBuff(buffData, 0);
    }

    public void Btn_PreviewHp(TMP_InputField value)
    {
        var v = _currentHp + int.Parse(value.text);
        if (v > 100) v = 100;
        if (v < 0) v = 0;
        statsUI.PreviewHp(_currentHp, v, _maxHp);
    }

    public void Btn_PreviewShield(TMP_InputField value)
    {
        var v = _currentShield + int.Parse(value.text);
        if (v < 0) v = 0;
        statsUI.PreviewShield(_currentShield, v);
    }

    public void Btn_CancelPreviewHp()
    {
        statsUI.CancelPreviewHp();
    }

    public void Btn_CancelPreviewShield()
    {
        statsUI.CancelPreviewShield();
    }

    public void Btn_CancelPreviewAll()
    {
        Btn_CancelPreviewHp();
        Btn_CancelPreviewShield();
    }
    #endregion
}
