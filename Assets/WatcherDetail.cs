using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DefaultNamespace;

public class WatcherDetail : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _detail_txt;
    [SerializeField] TextMeshProUGUI _num_txt;
    [SerializeField] Image _icon_img;
    [SerializeField] Image _highlight_img;


    [SerializeField] Color _s_color;
    [SerializeField] Color _m_color;
    [SerializeField] Color _l_color;

    [SerializeField] Sprite _s_sprite;
    [SerializeField] Sprite _m_sprite;
    [SerializeField] Sprite _l_sprite;



    public void SetDetail(Size size, string text)
    {
        if (size == Size.S)
        {
            _icon_img.color = _s_color;
            _icon_img.sprite = _s_sprite;
        }
        else if(size == Size.M)
        {
            _icon_img.color = _m_color;
            _icon_img.sprite = _m_sprite;
        }
        else
        {
            _icon_img.color = _l_color;
            _icon_img.sprite = _l_sprite;
        }

        _detail_txt.text = text;
        _icon_img.gameObject.SetActive(true);
        _num_txt.gameObject.SetActive(false);
    }

    public void SetDetail(int size, string text)
    {
        _detail_txt.text = text;
        _icon_img.gameObject.SetActive(false);
        _num_txt.text = size.ToString();
        _num_txt.gameObject.SetActive(true);
    }

    public void Highlight()
    {
        _highlight_img.gameObject.SetActive(true);
    }

    public void UnHighlight()
    {
        _highlight_img.gameObject.SetActive(false);
    }
}
