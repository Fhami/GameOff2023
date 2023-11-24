using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefaultNamespace;

public class WatcherUITester : MonoBehaviour
{
    [SerializeField] WatcherUI _watcherUI;

    public void Btn_Small()
    {
        _watcherUI.AddDetail(Size.S, "Shield -2 every turn");
    }

    public void Btn_Big()
    {
        _watcherUI.AddDetail(Size.L, "Attack area");
    }

    public void Btn_Normal()
    {
        _watcherUI.AddDetail(Size.M, "Focus asdasdasdasdasd");
    }

    public void Btn_4()
    {
        _watcherUI.AddDetail(4, "Focus asdasdasdasdasd");
    }

    public void Btn_6()
    {
        _watcherUI.AddDetail(6, "First 6 skill activate ");
    }

    public void Btn_6v2()
    {
        _watcherUI.AddDetail(6, "Second first 6 skill activate");
    }

    public void Btn_Show()
    {
        _watcherUI.Show();
    }

    public void Btn_Hide()
    {
        _watcherUI.Hide();
    }

}
