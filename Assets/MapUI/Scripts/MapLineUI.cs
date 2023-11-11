using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Radishmouse;
using UnityEngine.UI;

public class MapLineUI : MonoBehaviour
{
    public RectTransform _rect;
    [SerializeField] UILineRenderer _lineRenderer;

    public void SetLine(Vector2 start, Vector2 end)
    {
        Vector2[] p = new Vector2[]{ start, end };
        _lineRenderer.points = p;
    }

}
