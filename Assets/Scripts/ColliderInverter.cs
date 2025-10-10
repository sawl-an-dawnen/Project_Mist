using System.Collections;
using UnityEngine;

public class ColliderInverter : MonoBehaviour, Invertable
{
    private Collider2D[] colliders;
    private bool inverted = false;

    void Awake()
    {
        // Cache all colliders on this GameObject
        colliders = GetComponents<Collider2D>();
    }

    //void Start() {
    //    if (startDisabled) {
    //        foreach (var col in colliders)
    //        {
    //            if (col != null)
    //                col.enabled = false;
    //        }
    //    }
    //}

    public void Invert()
    {
        if (gameObject.activeSelf == true)
        {
            inverted = !inverted;
            InvertColliders();
        }
    }

    private void InvertColliders()
    {
        foreach (var col in colliders)
        {
            if (col != null)
                col.enabled = !col.enabled;
        }
    }
}
