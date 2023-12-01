using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class ResultUI : MonoBehaviour
{
    [SerializeField] Canvas main_canvas;
    [SerializeField] Canvas main_canvasGroup;
    [SerializeField] GameObject win_parent;
    [SerializeField] GameObject lost_parent;
    [SerializeField] Action onClickGoNext;
    [SerializeField] Action onClick_BackToMenu;

    public Action OnClick_GoNext { get => onClickGoNext; set => onClickGoNext = value; }
    public Action OnClick_BackToMenu { get => onClick_BackToMenu; set => onClick_BackToMenu = value; }
    

    public void ShowWin()
    {
        win_parent.SetActive(true);
        lost_parent.SetActive(false);
        Show();

    }

    public void ShowLose()
    {
        win_parent.SetActive(false);
        lost_parent.SetActive(true);
        Show();
    }

    void Show()
    {
        main_canvas.gameObject.SetActive(true);
    }

    void Hide()
    {
        main_canvas.gameObject.SetActive(false);
    }

    public void Btn_PlayAgain()
    {
        if (OnClick_GoNext != null) OnClick_GoNext.Invoke();
        Hide();
    }

    public void Btn_BackToMenu()
    {
        if (OnClick_BackToMenu != null) OnClick_BackToMenu.Invoke();
        Hide();
    }
}
