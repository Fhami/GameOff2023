using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DefaultNamespace;
using MPUIKIT;

public class TagSizeUI : MonoBehaviour
{
    [SerializeField] MPImage _image;
    [SerializeField] TextMeshProUGUI size_txt;
    [SerializeField] Image bgColor;
    [SerializeField] Color _s_color;
    [SerializeField] Color _m_color;
    [SerializeField] Color _l_color;
    [SerializeField] GameObject _focus_efx;

    public void Set(Size size, int value)
    {
        size_txt.text = value.ToString();

        if(size== Size.L)
        {
            bgColor.color = _l_color;
        }
        else if (size == Size.M)
        {
            bgColor.color = _m_color;
        }
        else
        {
            bgColor.color = _s_color;
        }

        size_txt.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void Highlight(bool value)
    {
        if (value)
        {
            _image.OutlineWidth = 2;
            _image.transform.localScale = new Vector3(1.1f, 1.1f, 1);
            _focus_efx?.gameObject.SetActive(true);
        }
        else
        {
            _image.OutlineWidth = 0;
            _image.transform.localScale = new Vector3(1, 1, 1);
            _focus_efx?.gameObject.SetActive(false);
        }
    }

}
