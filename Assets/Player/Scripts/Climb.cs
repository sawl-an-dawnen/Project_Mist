using UnityEngine;

public class Climb : MonoBehaviour
{
    public float climbSpeed = 5f; // Speed at which the player climbs
    public LayerMask platformLayer; // Layer mask for the one-way platforms
    //public string interactButton = "Interact"; // Button for interacting with the ladder

    private bool isClimbing = false; // Whether the player is currently climbing
    private Rigidbody2D rb; // Reference to the player's Rigidbody2D
    private Collider2D ladderCollider; // Current ladder the player is on
    private Collider2D playerCollider; // Player's collider
    private Animator animator;
    private Movement move;
    private GameManager gameManager;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        move = GetComponent<Movement>();
        gameManager = GameManager.Instance;
    }

    void Update()
    {
        // Check for climbing activation
        if (ladderCollider != null)
        {
            // Press up on stick or interact button
            if ((Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f || Input.GetKeyDown(KeyCode.E)) && !move.Interacting() && gameManager.InControl() && !gameManager.Paused())
            {
                animator.SetBool("Climbing", true);
                isClimbing = true;
                rb.gravityScale = 0f; // Disable gravity while climbing

                // Ignore collisions with platforms while climbing
                Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Platform"), true);
            }
        }

        // Cancel climbing when leaving the ladder
        if (isClimbing && ladderCollider == null)
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

            // Optional: Dismount when pressing down
            if (Input.GetAxis("Vertical") < -0.1f && ladderCollider == null || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
            {
                StopClimbing();
            }
        }
    }

    private void StopClimbing()
    {
        isClimbing = false;
        animator.SetBool("Climbing", false);
        animator.SetFloat("Vertical Climb", 0f);
        animator.SetFloat("Horizontal Climb", 0f);
        rb.gravityScale = 1f; // Restore gravity

        // Re-enable collisions with platforms
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Platform"), false);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the player enters a ladder
        if (collision.CompareTag("Ladder"))
        {
            ladderCollider = collision;
        }
    }

    public bool Climbing() 
    {
        return isClimbing;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Check if the player exits a ladder
        if (collision.CompareTag("Ladder"))
        {
            ladderCollider = null;
            StopClimbing();
        }
    }
}