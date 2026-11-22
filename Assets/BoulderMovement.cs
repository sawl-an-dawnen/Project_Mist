using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BoulderMovement : MonoBehaviour
{
    [Header("Rolling Settings")]
    public float rollTorque = 20f;
    public float maxAngularSpeed = 300f;

    private Rigidbody2D rb;
    private Movement playerMovement;

    private bool playerOnTop = false;
    private bool touchingPlayer = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!touchingPlayer)
        {
            ClearPlayer();
            return;
        }

        touchingPlayer = false;

        if (!playerOnTop || playerMovement == null)
            return;

        float horizontalInput = playerMovement.GetMoveInput().x;

        rb.AddTorque(
            horizontalInput * rollTorque,
            ForceMode2D.Force
        );

        rb.angularVelocity = Mathf.Clamp(
            rb.angularVelocity,
            -maxAngularSpeed,
            maxAngularSpeed
        );
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        Movement movement = collision.gameObject.GetComponent<Movement>();

        if (movement == null)
            return;

        touchingPlayer = true;

        bool standingOnTop = false;

        // Confirm the player is physically standing on the boulder
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y < -0.5f)
            {
                standingOnTop = true;
                break;
            }
        }

        playerOnTop = standingOnTop;
        playerMovement = movement;
        movement.SetCurrentBoulder(standingOnTop ? rb : null); ;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        ClearPlayer();
    }

    private void ClearPlayer()
    {
        if (playerMovement != null)
        {
            playerMovement.SetCurrentBoulder(null);
        }

        playerMovement = null;
        playerOnTop = false;
        touchingPlayer = false;
    }
}