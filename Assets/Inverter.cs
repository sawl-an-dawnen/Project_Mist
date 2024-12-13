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
    public void OnInvert() 
    {
        Debug.Log("INVERT!");
        Invertable[] objs = GameObject.FindObjectsOfType<MonoBehaviour>(true).OfType<Invertable>().ToArray();

        foreach (Invertable obj in objs) 
        {
            obj.Invert();
        }
    }

}