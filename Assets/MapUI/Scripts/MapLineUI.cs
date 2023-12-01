using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Radishmouse;
using UnityEngine.UI;

public class MapLineUI : MonoBehaviour
{
    public RectTransform _rect;
    [SerializeField] UILineRenderer _lineRenderer;
    [SerializeField] Material _line_material;

    public void SetLine(Vector2 start, Vector2 end)
    {
        Vector2[] p = new Vector2[]{ start, end };
        _lineRenderer.points = p;
        _lineRenderer.material = _line_material;
    }

}
