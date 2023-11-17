using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterGraphic : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    Vector2 defaultPosition;
    [SerializeField] Vector2 defaultScale = new Vector2(1, 1);
    [SerializeField] Vector2 scaleModifier = new Vector2(0.1f, 0.1f);
    

    void Start()
    {
        
    }

    public void ScaleUp(int step)
    {

    }

    public void ScaleDown(int step)
    {

    }
}
