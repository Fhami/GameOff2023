using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;

public class DragableObject : MonoBehaviour
{
    /// <summary>
    /// Use this to move when drag instead of directly moving card so we can easily move back when cancel
    /// </summary>
    //[SerializeField] private Transform target;

    [SerializeField] private MMF_Player draggingFeedback;
    [SerializeField] private MMF_Player stopDragFeedback;

    //Set this to false when dropped card on target to prevent card moving back to origin
    public bool dropped = false;
    [SerializeField] private bool isDragging = false;
    private Vector3 offset;
    private Vector3 origin;

    private void OnMouseDown()
    {
        origin = transform.localPosition;
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        isDragging = true;
        dropped = false;
    }

    private void OnMouseDrag()
    {
        if (!isDragging) return;

        //Play feedback
        if (draggingFeedback)
        {
            if (!draggingFeedback.IsPlaying)
            {
                draggingFeedback.PlayFeedbacks();
            }
        }
        
        // Calculate the new position based on mouse movement
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;

        // Ensure that the object stays on the x and y plane
        newPosition.z = 0f;

        // Update the object's position
        transform.position = newPosition;
    }

    private void OnMouseUp()
    {
        if (draggingFeedback)
        {
            draggingFeedback.StopFeedbacks();
        }

        if (stopDragFeedback)
        {
            stopDragFeedback.StopFeedbacks();
            stopDragFeedback.PlayFeedbacks();
        }
        
        isDragging = false;
    }

    public void MoveToOrigin()
    {
        transform.DOLocalMove(origin, 0.3f);
    }
}
