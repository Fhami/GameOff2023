using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public abstract class BaseInputFeelFeedback : MonoBehaviour
{
    public bool isLock = false;
    
    [SerializeField] private MMF_Player hoverInPlayer;
    [SerializeField] private MMF_Player hoverOutPlayer;
    [SerializeField] private MMF_Player pointerDownPlayer;
    [SerializeField] private MMF_Player pointerUpPlayer;
    [SerializeField] private MMF_Player lockClickPlayer;
    
    protected RectTransform RectTransform => (RectTransform)transform;

    protected MMPositionShaker[] PositionShakers;
    protected MMRotationShaker[] RotationShakers;
    protected MMScaleShaker[] ScaleShakers;

    [SerializeField] protected bool isDebug;
    
    /// <summary>
    /// Use this to iterate every MMF_Player instead of calling it one by one 
    /// </summary>
    protected IEnumerable<MMF_Player> AllMmfPlayer
    {
        get
        {
            if (hoverInPlayer)
                yield return hoverInPlayer;

            if (hoverOutPlayer)
                yield return hoverOutPlayer;

            if (pointerDownPlayer)
                yield return pointerDownPlayer;

            if (pointerUpPlayer)
                yield return pointerUpPlayer;

            if (lockClickPlayer)
                yield return lockClickPlayer;
        }
    }
    
    private void Start()
    {
        //Need manually get each shakers because MMShaker doesn't have Mode and TargetRectTransform
        
        PositionShakers = GetComponentsInChildren<MMPositionShaker>();
        RotationShakers = GetComponentsInChildren<MMRotationShaker>();
        ScaleShakers = GetComponentsInChildren<MMScaleShaker>();
        
        foreach (var mmfPlayer in AllMmfPlayer)
        {
            foreach (var feedback in mmfPlayer.FeedbacksList)
            {
                feedback.AutomatedTargetAcquisition.Mode = MMFeedbackTargetAcquisition.Modes.Parent;
                feedback.ForceAutomateTargetAcquisition();
            }
        }
    }

    protected virtual void Init()
    {
        
    }

    protected void HoverIn()
    {
        if (isDebug)
            Debug.Log("HoverIn");
        
        if (!hoverInPlayer) return;

        StopAllFeedbacks();

        hoverInPlayer.PlayFeedbacks();
    }

    protected void HoverOut()
    {
        if (isDebug)
            Debug.Log("HoverOut");
        
        if (!hoverOutPlayer) return;

        StopAllFeedbacks();

        hoverOutPlayer.PlayFeedbacks();
    }

    protected void PointerDown()
    {
        if (isDebug)
            Debug.Log("PointerDown");
        
        if (!pointerDownPlayer) return;

        StopAllFeedbacks();

        pointerDownPlayer.PlayFeedbacks();
    }

    protected void PointerUp()
    {
        if (isDebug)
            Debug.Log("PointerUp");
        
        if (!pointerUpPlayer) return;

        StartCoroutine(IEPointerUp());
    }

    protected void LockClick()
    {
        if (isDebug)
            Debug.Log("LockClick");
        
        if (!lockClickPlayer) return;

        StopAllFeedbacks();

        lockClickPlayer.PlayFeedbacks();
    }

    protected IEnumerator IEPointerUp()
    {
        if (pointerDownPlayer)
            yield return new WaitUntil(() => !pointerDownPlayer.IsPlaying);

        pointerUpPlayer.PlayFeedbacks();
    }

    protected void StopAllFeedbacks()
    {
        foreach (var mmfPlayer in AllMmfPlayer)
        {
            StopFeedback(mmfPlayer);
        }
    }

    protected void StopFeedback(MMF_Player player)
    {
        if (!player) return;

        player.StopFeedbacks();
    }
}