using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorInverter : MonoBehaviour, Invertable
{
    public Color color01;
    public Color color02;
    private SpriteRenderer sprite;
    private bool inverted = false;


    // Start is called before the first frame update
    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = color01;
    }

    public void Invert() 
    {
        if (inverted)
        {
            sprite.color = color01;
        }
        else
        {
            sprite.color = color02;
        }
        inverted = !inverted;
    }
}
