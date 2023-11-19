using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICardTarget
{
    public GameObject GameObject { get; }

    public void Highlight(bool _value);
    public void HighlightSelected(bool _value);
}
