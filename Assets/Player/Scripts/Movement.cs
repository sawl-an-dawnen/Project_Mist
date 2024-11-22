using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isGrounded;
    private bool jumpPressed;

    private Animator animator;
    private Vector3 playerScale;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerScale = gameObject.transform.localScale;
    }
    void Update()
    {
        // Movement animation logic and param
        if (moveInput.magnitude > 0)  {
            animator.SetBool("Move", true);
            animator.SetFloat("Horizontal Input", moveInput.x);
            Debug.Log(moveInput.x);
            if (moveInput.x > 0) {
                gameObject.transform.localScale = new Vector3(playerScale.x, playerScale.y, playerScale.z);
            }
            else if (moveInput.x < 0) {
                gameObject.transform.localScale = new Vector3(-playerScale.x, playerScale.y, playerScale.z);
            }
        }
        else {
            animator.SetBool("Move", false);
        }


        // Set jump and fall animation param
        animator.SetFloat("Vertical Velocity", rb.velocity.y);

        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded)   {
            animator.SetBool("Grounded", true);
        }
        else {
            animator.SetBool("Grounded", false);
        }

        // Jump logic
        if (jumpPressed && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        jumpPressed = false; // Reset jump flag
    }

    void FixedUpdate()
    {
        // Horizontal movement
        rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
    }

    public void OnMove(InputValue value)
    {   
        moveInput = value.Get<Vector2>();
        //Debug.Log("OnMove Called");
        //Debug.Log(moveInput);
    }

    public void OnJump()
    {
        //Debug.Log("OnMove Called");
        jumpPressed = true;

    }

    public bool checkGrounded() 
    {
        if (isGrounded) return true;
        return false;
    }

    void OnDrawGizmosSelected()
    {
        // Draw ground check radius for debugging
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
