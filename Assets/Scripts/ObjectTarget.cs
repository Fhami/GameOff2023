using System.Collections;
using System.Collections.Generic;
using EPOOutline;
using UnityEngine;

public class ObjectTarget : MonoBehaviour, ICardTarget
{
    public GameObject GameObject => gameObject;
    [SerializeField] private Outlinable outlinable;
    
    public void Highlight(bool _value)
    {
        outlinable.enabled = _value;
    }

    public void HighlightSelected(bool _value)
    {
        if (_value)
            outlinable.OutlineParameters.FillPass.Shader =
                Resources.Load<Shader>("Easy performant outline/Shaders/Fills/ColorFill");
        else
            outlinable.OutlineParameters.FillPass.Shader = null;
    }
}
