using System.Collections;
using UnityEngine;

public class ColliderInverter : MonoBehaviour, IInvertable
{
    private Collider2D[] colliders;
    private Rigidbody2D rb;
    private bool inverted = false;

    void Awake()
    {
        // Cache all colliders on this GameObject
        colliders = GetComponents<Collider2D>();

        // Cache Rigidbody2D if it exists
        rb = GetComponent<Rigidbody2D>();
    }

    public void Invert()
    {
        if (gameObject.activeSelf)
        {
            inverted = !inverted;
            InvertCollidersAndRigidbody();
        }
    }

    private void InvertCollidersAndRigidbody()
    {
        bool collidersEnabled = false;

        // Toggle all colliders
        foreach (var col in colliders)
        {
            if (col != null)
            {
                col.enabled = !col.enabled;
                collidersEnabled |= col.enabled; // check if any collider is still enabled
            }
        }

        // If there's a Rigidbody2D, simulate physics only if colliders are enabled
        if (rb != null)
        {
            if (collidersEnabled)
            {
                rb.simulated = true;
                rb.isKinematic = false;
            }
            else
            {
                rb.simulated = false;
                rb.isKinematic = true;
            }
        }
    }
}
