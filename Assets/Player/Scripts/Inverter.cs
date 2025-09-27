using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

interface Invertable
{
    public void Invert();
}

public class Inverter : MonoBehaviour
{
    GameManager gameManager;
    public void Awake()
    {
        gameManager = GameManager.Instance;
    }

    public void OnInvert() 
    {
        if (gameManager.InControl() && !gameManager.Paused() && gameManager.CanInvert()) {
            Invertable[] objs = GameObject.FindObjectsOfType<MonoBehaviour>(true).OfType<Invertable>().ToArray();
            gameManager.Invert();

            foreach (Invertable obj in objs)
            {
                obj.Invert();
            }
        }
    }
}