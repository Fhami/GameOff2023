using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableLine : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;

    private Vector3[] points = new Vector3[2];

    public void Cancel()
    {
        lineRenderer.enabled = false;
    }
    
    private void OnMouseDown()
    {
        points[0] = transform.position;
        points[0].z = 0;
    }

    private void OnMouseDrag()
    {
        lineRenderer.enabled = true;
        points[1] = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // var _direction  = (points[1] - points[0]).normalized;
        // points[1] += (_direction * 1f);
        points[1].z = 0;

        lineRenderer.SetPositions(points);
        
    }

    private void OnMouseUp()
    {
        Cancel();
    }
}
