using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeUITester : MonoBehaviour
{
    [SerializeField] SizeUI _sizeUI;

    #region Test Button Function

    public void Btn_Increase()
    {
        _sizeUI.SetSize(8, SizeEffectType.Increase);
    }
    public void Btn_Decrease()
    {
        _sizeUI.SetSize(2, SizeEffectType.Decrease);
    }
    public void Btn_Set()
    {
        _sizeUI.SetSize(5);
    }

    public void Btn_Set01()
    {
        _sizeUI.InitSizeUI(5,3,8,0,-1);

        _sizeUI.SetSizeSaperation(3, 0, 10);
        _sizeUI.SetSizeSaperation(7, 0, 10);

        _sizeUI.SetTag(DefaultNamespace.Size.S, 2, 0, 10);
        _sizeUI.SetTag(DefaultNamespace.Size.M, 5, 0, 10);
        _sizeUI.SetTag(DefaultNamespace.Size.L,8, 0, 10);

    }

    public void Btn_Set02()
    {
        _sizeUI.InitSizeUI(5, 3, 8, 0, -1);

        _sizeUI.SetSizeSaperation(3, 0, 10);
        _sizeUI.SetSizeSaperation(7, 0, 10);

        _sizeUI.SetTag(DefaultNamespace.Size.S, 2, 0, 10);
        _sizeUI.SetTag(DefaultNamespace.Size.M, 5, 0, 10);
        _sizeUI.SetTag(DefaultNamespace.Size.L, 8, 0, 10);

    }

    public void GoToSize01()
    {
        _sizeUI.GoToSize(5, 3, 5, 8, 0, 10);
    }

    public void GoToSize02()
    {
        _sizeUI.GoToSize(0, 3, 5, 8, 0, 10);
    }


    #endregion



}
