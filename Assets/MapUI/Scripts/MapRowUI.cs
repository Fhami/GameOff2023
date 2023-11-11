using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapRowUI : MonoBehaviour
{
    public RectTransform _rect;
    public VerticalLayoutGroup _vertical_layout_group;
    public List<MapNodeUI> _nodes;


    void Start()
    {
        
    }

    public void offsetChilds()
    {
        foreach (var node in _nodes)
        {
            float x, y;
            x = Random.Range(-70, 70);
            y = Random.Range(-50, -50);
            node.Parent_img.rectTransform.anchoredPosition = 
                new Vector2(node.Parent_img.rectTransform.anchoredPosition.x + x,
                node.Parent_img.rectTransform.anchoredPosition.y + y);
        }
    }

}
