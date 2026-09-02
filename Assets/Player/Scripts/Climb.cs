using System.Collections.Generic;
using UnityEngine;

public class Climb : MonoBehaviour
{
    public float climbSpeed = 5f;
    public LayerMask platformLayer;

    private bool isClimbing = false;
    private float startingGravityScale;

    private Rigidbody2D rb;
    private Animator animator;
    private Movement move;
    private GameManager gameManager;

    // Keep track of every ladder collider currently overlapping the player
    private HashSet<Collider2D> ladderColliders = new HashSet<Collider2D>();

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        move = GetComponent<Movement>();
        gameManager = GameManager.Instance;

        startingGravityScale = rb.gravityScale;
    }

    void Update()
    {
        // Check for climbing activation
        if (ladderColliders.Count > 0)
        {
            if ((Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f || Input.GetKeyDown(KeyCode.E))
                && !move.Interacting()
                && gameManager.InControl()
                && !gameManager.Paused())
            {
                StartClimbing();
            }
        }

        // If we are climbing but no longer touching ANY ladder,
        // stop climbing.
        if (isClimbing && ladderColliders.Count == 0)
        {
            StopClimbing();
        }

        // Handle climbing
        if (isClimbing)
        {
            float verticalInput = Input.GetAxis("Vertical");
            float horizontalInput = Input.GetAxis("Horizontal");

            animator.SetFloat("Vertical Climb", Mathf.Abs(rb.velocity.y));
            animator.SetFloat("Horizontal Climb", Mathf.Abs(rb.velocity.x));

            if (Mathf.Abs(verticalInput) > Mathf.Abs(horizontalInput))
            {
                rb.velocity = new Vector2(0f, verticalInput * climbSpeed);
            }
            else
            {
                rb.velocity = new Vector2(horizontalInput * climbSpeed, 0f);
            }

            // Dismount when pressing down while no longer touching a ladder,
            // or when pressing E / Space.
            if ((Input.GetAxis("Vertical") < -0.1f && ladderColliders.Count == 0)
                || Input.GetKeyDown(KeyCode.E)
                || Input.GetKeyDown(KeyCode.Space))
            {
                StopClimbing();
            }
        }
    }

    private void StartClimbing()
    {
        if (isClimbing)
            return;

        Debug.Log("Started climbing");

        isClimbing = true;

        startingGravityScale = rb.gravityScale;

        rb.gravityScale = 0f;

        animator.SetBool("Climbing", true);

        // Ignore collisions with platforms while climbing
        Physics2D.IgnoreLayerCollision(
            gameObject.layer,
            LayerMask.NameToLayer("Platform"),
            true
        );
    }

    private void StopClimbing()
    {
        if (!isClimbing)
            return;

        Debug.Log("Stopped climbing");

        isClimbing = false;

        animator.SetBool("Climbing", false);
        animator.SetFloat("Vertical Climb", 0f);
        animator.SetFloat("Horizontal Climb", 0f);

        // Restore gravity
        rb.gravityScale = startingGravityScale;

        // Re-enable collisions with platforms
        Physics2D.IgnoreLayerCollision(
            gameObject.layer,
            LayerMask.NameToLayer("Platform"),
            false
        );
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            Debug.Log("Entered ladder: " + collision.name);

            ladderColliders.Add(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            Debug.Log("Exited ladder: " + collision.name);

            ladderColliders.Remove(collision);

            // Only stop climbing if we have completely
            // left ALL ladder colliders.
            if (ladderColliders.Count == 0)
            {
                StopClimbing();
            }
        }
    }

    public bool Climbing()
    {
        return isClimbing;
    }
}
