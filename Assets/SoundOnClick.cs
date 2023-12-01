using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SoundOnClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] AudioClip clip;

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySFX(clip);
    }
}
