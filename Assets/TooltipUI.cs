using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoreMountains;


public class TooltipUI : MonoBehaviour
{
    public enum Side { TopLeft, TopRight, BotLeft, BotRight }

    [SerializeField] RectTransform _canvas;
    [SerializeField] Image _content;
    [SerializeField] TextMeshProUGUI _header_txt;
    [SerializeField] TextMeshProUGUI _tooltip_txt;

    /// <summary>
    /// Show Tooltip
    /// </summary>
    /// <param name="position"> Mouse position</param>
    /// <param name="text"> Detail </param>
    /// <param name="side"> Graphic side from mouse</param>
    public void Show(string text, Vector3 position, Side side)
    {
        Vector2 p;
        
        if(side== Side.BotRight)//Pivot TopLeft -> Graphic on Botright.
        {
            p = new Vector2(0, 1);
        }
        else if (side == Side.BotLeft)//Pivot TopRight  -> Graphic on Botleft.
        {
            p = new Vector2(1, 1);
        }
        else if (side == Side.TopRight)//Pivot BotLeft  -> Graphic on Topright.
        {
            p = new Vector2(0, 0);
        }
        else//Pivot BotRight -> Graphic on Topleft.
        {
            p = new Vector2(1, 0);
        }

        _canvas.position = position;
        _canvas.pivot = p;
        _content.rectTransform.anchorMin = p;
        _content.rectTransform.anchorMax = p;
        _content.rectTransform.pivot = p;
        _tooltip_txt.text = text;

        _header_txt.gameObject.SetActive(false);
        _content.gameObject.SetActive(true);


        LayoutRebuilder.ForceRebuildLayoutImmediate(_content.rectTransform);
    }

    public void Show(string header, string text, Vector3 position, Side side)
    {
        Vector2 p;

        if (side == Side.BotRight)//Pivot TopLeft -> Graphic on Botright.
        {
            p = new Vector2(0, 1);
        }
        else if (side == Side.BotLeft)//Pivot TopRight  -> Graphic on Botleft.
        {
            p = new Vector2(1, 1);
        }
        else if (side == Side.TopRight)//Pivot BotLeft  -> Graphic on Topright.
        {
            p = new Vector2(0, 0);
        }
        else//Pivot BotRight -> Graphic on Topleft.
        {
            p = new Vector2(1, 0);
        }

        _canvas.position = position;
        _canvas.pivot = p;
        _content.rectTransform.anchorMin = p;
        _content.rectTransform.anchorMax = p;
        _content.rectTransform.pivot = p;
        _tooltip_txt.text = text;
        _header_txt.text = header;

        _header_txt.gameObject.SetActive(true);
        _content.gameObject.SetActive(true);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_content.rectTransform);
    }

    //IEnumerator ieShow(string text, Vector3 position, Side side)
    //{
    //    Vector2 p;

    //    if (side == Side.BotRight)//Pivot TopLeft -> Graphic on Botright.
    //    {
    //        p = new Vector2(0, 1);
    //    }
    //    else if (side == Side.BotLeft)//Pivot TopRight  -> Graphic on Botleft.
    //    {
    //        p = new Vector2(1, 1);
    //    }
    //    else if (side == Side.TopRight)//Pivot BotLeft  -> Graphic on Topright.
    //    {
    //        p = new Vector2(0, 0);
    //    }
    //    else//Pivot BotRight -> Graphic on Topleft.
    //    {
    //        p = new Vector2(1, 0);
    //    }

    //    _canvas.position = position;
    //    _canvas.pivot = p;
    //    _content.rectTransform.anchorMin = p;
    //    _content.rectTransform.anchorMax = p;
    //    _content.rectTransform.pivot = p;
    //    _tooltip_txt.text = text;
        
    //    _header_txt.gameObject.SetActive(false);
    //    _content.gameObject.SetActive(true);
    //}

    /// <summary>
    /// Hide tooltip
    /// </summary>
    public void Hide()
    {
        _content.gameObject.SetActive(false);
    }

    public void Btn_Tooltip()
    {
        Show( "asdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasd" +
            "asdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasd",new Vector2(0, 0), Side.BotRight);
    }
}
