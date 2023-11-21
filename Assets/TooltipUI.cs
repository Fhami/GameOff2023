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
    [SerializeField] TextMeshProUGUI _tooltip_txt;

    /// <summary>
    /// Show Tooltip
    /// </summary>
    /// <param name="position"> Mouse position</param>
    /// <param name="text"> Detail </param>
    /// <param name="pivot"> Graphic side from mouse</param>
    public void Show(Vector3 position, string text, Side pivot)
    {
        Vector2 p;
        
        if(pivot== Side.BotRight)//Pivot TopLeft -> Graphic on Botright.
        {
            p = new Vector2(0, 1);
        }
        else if (pivot == Side.BotLeft)//Pivot TopRight  -> Graphic on Botleft.
        {
            p = new Vector2(1, 1);
        }
        else if (pivot == Side.TopRight)//Pivot BotLeft  -> Graphic on Topright.
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

        _content.gameObject.SetActive(true);
    }
    /// <summary>
    /// Hide tooltip
    /// </summary>
    public void Hide()
    {
        _content.gameObject.SetActive(false);
    }

    public void Btn_Tooltip()
    {
        Show(new Vector2(0, 0), "asdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasd" +
            "asdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasd", Side.BotRight);
    }
}
