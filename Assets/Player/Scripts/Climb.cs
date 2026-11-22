using System.Collections.Generic;
using UnityEngine;

public class Climb : MonoBehaviour
{
    [Header("Climbing Settings")]
    public float climbSpeed = 5f;
    public LayerMask platformLayer;

    [Header("Animation Settings")]
    public float directionSwitchThreshold = 0.2f;
    public float inputDeadzone = 0.1f;

    private bool isClimbing = false;
    private bool climbingVertically = true;

    private float startingGravityScale;

    private float verticalInput;
    private float horizontalInput;

    private Rigidbody2D rb;
    private Animator animator;
    private Movement move;
    private GameManager gameManager;

    // Track all ladder colliders currently overlapping the player
    private HashSet<Collider2D> ladderColliders =
        new HashSet<Collider2D>();

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
        // Remove invalid ladder colliders
        ladderColliders.RemoveWhere(c =>
            c == null ||
            !c.enabled ||
            !c.gameObject.activeInHierarchy
        );

        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");

        // Stop climbing when leaving all climb zones
        if (ladderColliders.Count == 0)
        {
            if (isClimbing)
            {
                StopClimbing();
            }
        }

        // Press Space to stop climbing
        if (isClimbing && Input.GetKeyDown(KeyCode.Space))
        {
            StopClimbing();
            return;
        }

        // Start climbing
        if (!isClimbing &&
            !GetComponent<Mantle>().MantleEnabled &&
            ladderColliders.Count > 0 &&
            !move.Interacting() &&
            gameManager.InControl() &&
            !gameManager.Paused())
        {
            if (Mathf.Abs(verticalInput) > inputDeadzone ||
                Input.GetKeyDown(KeyCode.E))
            {
                StartClimbing();
            }
        }

        if (!isClimbing)
            return;

        // Pause climbing movement
        if (!gameManager.InControl() || gameManager.Paused())
        {
            verticalInput = 0f;
            horizontalInput = 0f;

            UpdateClimbAnimation(0f, 0f);
            return;
        }

        UpdateClimbDirection();
    }

    void FixedUpdate()
    {
        if (!isClimbing)
            return;

        if (!gameManager.InControl() || gameManager.Paused())
        {
            rb.velocity = Vector2.zero;
            return;
        }

        float verticalSpeed = 0f;
        float horizontalSpeed = 0f;

        if (climbingVertically)
        {
            verticalSpeed = verticalInput;

            rb.velocity = new Vector2(
                0f,
                verticalSpeed * climbSpeed
            );
        }
        else
        {
            horizontalSpeed = horizontalInput;

            rb.velocity = new Vector2(
                horizontalSpeed * climbSpeed,
                0f
            );
        }
    }

    private void UpdateClimbDirection()
    {
        float absVertical = Mathf.Abs(verticalInput);
        float absHorizontal = Mathf.Abs(horizontalInput);

        // Ignore tiny input fluctuations
        if (absVertical < inputDeadzone)
            absVertical = 0f;

        if (absHorizontal < inputDeadzone)
            absHorizontal = 0f;

        // Prevent rapid switching between directions
        if (climbingVertically)
        {
            if (absHorizontal >
                absVertical + directionSwitchThreshold)
            {
                climbingVertically = false;
            }
        }
        else
        {
            if (absVertical >
                absHorizontal + directionSwitchThreshold)
            {
                climbingVertically = true;
            }
        }

        float verticalSpeed = climbingVertically
            ? absVertical
            : 0f;

        float horizontalSpeed = climbingVertically
            ? 0f
            : absHorizontal;

        UpdateClimbAnimation(verticalSpeed, horizontalSpeed);
    }

    private void UpdateClimbAnimation(
        float verticalSpeed,
        float horizontalSpeed)
    {
        animator.SetInteger(
            "Climb Direction",
            climbingVertically ? 1 : 2
        );

        animator.SetFloat(
            "Vertical Climb",
            Mathf.Clamp01(verticalSpeed)
        );

        animator.SetFloat(
            "Horizontal Climb",
            Mathf.Clamp01(horizontalSpeed)
        );
    }

    private void StartClimbing()
    {
        if (isClimbing)
            return;

        Debug.Log("[Climb] Started climbing");

        isClimbing = true;

        // Select initial direction based on input
        climbingVertically =
            Mathf.Abs(verticalInput) >=
            Mathf.Abs(horizontalInput);

        // Disable gravity
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;

        animator.SetInteger(
            "Climb Direction",
            climbingVertically ? 1 : 2
        );

        animator.SetFloat("Vertical Climb", 0f);
        animator.SetFloat("Horizontal Climb", 0f);
        animator.SetBool("Climbing", true);

        // Ignore platform collisions while climbing
        int platformLayerIndex =
            LayerMask.NameToLayer("Platform");

        if (platformLayerIndex >= 0)
        {
            Physics2D.IgnoreLayerCollision(
                gameObject.layer,
                platformLayerIndex,
                true
            );
        }
    }

    private void StopClimbing()
    {
        if (!isClimbing)
            return;

        Debug.Log("[Climb] Stopped climbing");

        isClimbing = false;

        animator.SetBool("Climbing", false);
        animator.SetInteger("Climb Direction", 0);
        animator.SetFloat("Vertical Climb", 0f);
        animator.SetFloat("Horizontal Climb", 0f);

        // Restore original gravity
        rb.gravityScale = startingGravityScale;

        // Stop residual climbing movement
        rb.velocity = Vector2.zero;

        // Restore platform collisions
        int platformLayerIndex =
            LayerMask.NameToLayer("Platform");

        if (platformLayerIndex >= 0)
        {
            Physics2D.IgnoreLayerCollision(
                gameObject.layer,
                platformLayerIndex,
                false
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Ladder"))
            return;

        ladderColliders.Add(collision);

        Debug.Log(
            "[Climb] Entered ladder: " + collision.name
        );
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Ladder"))
            return;

        ladderColliders.Add(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Ladder"))
            return;

        ladderColliders.Remove(collision);

        Debug.Log(
            "[Climb] Exited ladder: " + collision.name
        );

        // Stop only after leaving every climb zone
        if (ladderColliders.Count == 0)
        {
            StopClimbing();
        }
    }

    private void OnDisable()
    {
        if (isClimbing)
        {
            StopClimbing();
        }

        ladderColliders.Clear();
    }

    public bool Climbing()
    {
        return isClimbing;
    }

    public void StopClimbingForMantle()
    {
        if (isClimbing)
        {
            StopClimbing();
        }
    }
}