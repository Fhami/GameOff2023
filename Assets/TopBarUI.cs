using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;

public class TopBarUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI turn_txt;
    [SerializeField] TextMeshProUGUI name_txt;
    [SerializeField] TextMeshProUGUI time_txt;
    [SerializeField] TextMeshProUGUI run_txt;


    public void SetName(string name)
    {
        name_txt.text = "";
    }

    public void SetTurn(int turn)
    {
        turn_txt.text = "Turn " + turn;
    }

    public void SetRun(int run)
    {
        run_txt.text = "Run " + run;
    }

    public void Update()
    {
        time_txt.text = DateTime.Now.ToString("HH:mm tt");
    }
}
