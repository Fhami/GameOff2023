using MoreMountains.Feedbacks;
using UnityEngine.EventSystems;

public class UIInputFeelFeedback : BaseInputFeelFeedback, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    protected override void Init()
    {
        base.Init();
        
        foreach(var pShaker in PositionShakers)
        {
            pShaker.Mode = MMPositionShaker.Modes.RectTransform;
            pShaker.TargetRectTransform = RectTransform;
        }
        
        foreach(var rShaker in RotationShakers)
        {
            rShaker.Mode = MMRotationShaker.Modes.RectTransform;
            rShaker.TargetRectTransform = RectTransform;
        }
        
        foreach(var sShaker in ScaleShakers)
        {
            sShaker.Mode = MMScaleShaker.Modes.RectTransform;
            sShaker.TargetRectTransform = RectTransform;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HoverIn();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HoverOut();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isLock)
            PointerDown();
        else
            LockClick();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isLock)
            PointerUp();
    }
}
