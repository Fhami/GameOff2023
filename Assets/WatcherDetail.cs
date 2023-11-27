using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DefaultNamespace;
using AYellowpaper.SerializedCollections;

public class WatcherDetail : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _detail_txt;
    [SerializeField] TextMeshProUGUI _num_txt;

    [Header("Icon")]
    [SerializeField] Image _icon;
    [SerializeField] Image _icon_img;
    [SerializeField] Image _highlight_img;

    [Header("Condition")]
    [SerializeField] Image _condition;
    [SerializeField] Image _condition_img;
    [SerializeField] TextMeshProUGUI _contition_value_txt;

    [Header("Color")]
    [SerializeField] Color _s_color;
    [SerializeField] Color _m_color;
    [SerializeField] Color _l_color;

    [SerializeField] Sprite _s_sprite;
    [SerializeField] Sprite _m_sprite;
    [SerializeField] Sprite _l_sprite;

    [SerializeField] SerializedDictionary<Comparison, Sprite> comparisonSprite;

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

        _icon.gameObject.SetActive(true);
        _condition.gameObject.SetActive(false);

        _icon_img.gameObject.SetActive(true);
        _num_txt.gameObject.SetActive(false);

    }

    public void SetDetail(int size, string text)
    {
        _icon.gameObject.SetActive(true);
        _condition.gameObject.SetActive(false);

        _detail_txt.text = text;
        _num_txt.text = size.ToString();

        _icon_img.gameObject.SetActive(false);
        _num_txt.gameObject.SetActive(true);

    }

    public void SetDetail(PropertyCondition codition, string text)
    {

        _icon.gameObject.SetActive(false);
        _condition.gameObject.SetActive(true);

        _condition_img.sprite = comparisonSprite[codition.propertyComparer.comparison];
        _contition_value_txt.text = codition.propertyComparer.value.ToString();

        _detail_txt.text = text;

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
