using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeUITester : MonoBehaviour
{
    [SerializeField] SizeUI _sizeUI;
    [SerializeField] int _currentSize = 0;
    [SerializeField] SizeSetting _setting;

    #region Test Button Function

    public void Btn_Set01()
    {
        _setting = new(3, 5, 7, 0, 10,0,-1);
        _setting.skills = new List<int>() { 2, 5, 8 };
        _currentSize = 5;
        _sizeUI.InitSizeUI(_currentSize, _setting);
    }

    public void Btn_Set02()
    {
        _setting = new(2, 5, 9, 0, 10,0,10);
        _setting.skills = new List<int>() { 1, 3, 8 };
        _currentSize = 8;
        _sizeUI.InitSizeUI(_currentSize, _setting);
    }

    public void GoToSize04()
    {
        var to = _currentSize + 1;
        if (to < 0) to = 0;
        if (to > 10) to = 10;
        _sizeUI.GoToSize(_currentSize, to);
        _currentSize = to;
    }

    public void GoToSize03()
    {
        var to = _currentSize - 1;
        if (to < 0) to = 0;
        if (to > 10) to = 10;
        _sizeUI.GoToSize(_currentSize, to);
        _currentSize = to;
    }

    public void GoToSize02()
    {
        var to = _currentSize +3;
        if (to < 0) to = 0;
        if (to > 10) to = 10;
        _sizeUI.GoToSize(_currentSize, to);
        _currentSize = to;
    }

    public void GoToSize01()
    {
        var to = _currentSize - 3;
        if (to < 0) to = 0;
        if (to > 10) to = 10;
        _sizeUI.GoToSize(_currentSize, to);
        _currentSize = to;


    }

    public void FocusSkill(int value)
    {
        _sizeUI.HighlightTag(value, true);
    }

    public void StopFocusSkill(int value)
    {
        _sizeUI.HighlightTag(value, false);
    }




    #endregion



}
