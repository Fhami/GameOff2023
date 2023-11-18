
using System;
using MoreMountains.Feedbacks;

public class ObjectInputFeelFeedback : BaseInputFeelFeedback
{
    protected override void Init()
    {
        base.Init();
        
        foreach(var pShaker in PositionShakers)
        {
            pShaker.Mode = MMPositionShaker.Modes.Transform;
            pShaker.TargetRectTransform = RectTransform;
        }
        
        foreach(var rShaker in RotationShakers)
        {
            rShaker.Mode = MMRotationShaker.Modes.Transform;
            rShaker.TargetRectTransform = RectTransform;
        }
        
        foreach(var sShaker in ScaleShakers)
        {
            sShaker.Mode = MMScaleShaker.Modes.Transform;
            sShaker.TargetRectTransform = RectTransform;
        }
    }

    private void OnMouseEnter()
    {
        HoverIn();
    }

    private void OnMouseDrag()
    {
        Dragging();
    }

    private void OnMouseExit()
    {
        HoverOut();
    }

    private void OnMouseDown()
    {
        if (!isLock)
            PointerDown();
        else
            LockClick();
    }

    private void OnMouseUp()
    {
        if (!isLock)
            PointerUp();
    }
}
