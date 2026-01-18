using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

interface IInvertable
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
            IInvertable[] objs = GameObject.FindObjectsOfType<MonoBehaviour>(true).OfType<IInvertable>().ToArray();
            gameManager.Invert();

            foreach (IInvertable obj in objs)
            {
                obj.Invert();
            }
        }
    }
}