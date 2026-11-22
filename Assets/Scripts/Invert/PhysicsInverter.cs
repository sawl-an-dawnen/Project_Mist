using UnityEngine;

public class PhysicsInverter : MonoBehaviour, IInvertable
{
    [Header("Inversion Settings")]
    public bool autoInvert = false;

    public float invertedMass;
    public float invertedLinearDrag;
    public float invertedAngularDrag;
    public float invertedGravityScale;

    private bool inverted = false;

    private readonly float[] originalParameters = new float[4];

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Save original physics properties
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
            // Restore original physics properties
            rb.mass = originalParameters[0];
            rb.drag = originalParameters[1];
            rb.angularDrag = originalParameters[2];

            // Always restore original gravity,
            // even when the object is being carried.
            rb.gravityScale = originalParameters[3];

            inverted = false;

            Debug.Log(
                "[PhysicsInverter] Restored normal physics: " +
                gameObject.name +
                " | Gravity: " + rb.gravityScale
            );
        }
        else
        {
            // Apply inverted physics properties
            rb.mass = invertedMass;
            rb.drag = invertedLinearDrag;
            rb.angularDrag = invertedAngularDrag;
            rb.gravityScale = invertedGravityScale;

            inverted = true;

            Debug.Log(
                "[PhysicsInverter] Applied inverted physics: " +
                gameObject.name +
                " | Gravity: " + rb.gravityScale
            );
        }
    }

    public bool IsInverted()
    {
        return inverted;
    }
}