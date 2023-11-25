using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DefaultNamespace;

public class TagSizeUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI size_txt;
    [SerializeField] Image bgColor;
    [SerializeField] Color _l_color;
    [SerializeField] Color _m_color;
    [SerializeField] Color _s_color;


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

}
