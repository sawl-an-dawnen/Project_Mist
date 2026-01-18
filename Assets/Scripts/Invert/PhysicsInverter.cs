using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PhysicsInverter : MonoBehaviour, IInvertable
{
    public bool autoInvert = false;

    public float invertedMass;
    public float invertedLinearDrag;
    public float invertedAngularDrag;
    public float invertedGravityScale;
    private bool inverted = false;
    private float[] originalParameters = new float[4];
    private Rigidbody2D rb;


    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalParameters[0] = rb.mass;
        originalParameters[1] = rb.drag;
        originalParameters[2] = rb.angularDrag;
        originalParameters[3] = rb.gravityScale;

        if (autoInvert)
        {
            invertedMass = originalParameters[0];
            invertedLinearDrag = originalParameters[1];
            invertedAngularDrag = originalParameters[2];
            invertedGravityScale = -originalParameters[3];
        }
    }

    public void Invert()
    {
        if (inverted)
        {
            rb.mass = originalParameters[0];
            rb.drag = originalParameters[1];
            rb.angularDrag = originalParameters[2];
            rb.gravityScale = originalParameters[3];
        }
        else
        {
            rb.mass = invertedMass;
            rb.drag = invertedLinearDrag;
            rb.angularDrag = invertedAngularDrag;
            rb.gravityScale = invertedGravityScale;
        }
        inverted = !inverted;
    }
}