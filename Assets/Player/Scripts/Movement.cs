using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float horizontalJumpMultiplyer = 5f;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public float fallingMaxSpeed = 3f;
    private float fallingSpeed;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private float velocityX = 0f;
    private float jumpYCoordinate;
    private bool isGrounded;
    private bool jumpPressed;
    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    private Animator animator;
    private Vector3 playerScale;
    private float moveSpeedMultiplier = 1f;

    private Grab grabScript;
    private Mantle mantle;
    private Climb climb;
    private Interactable interactable;

    private GameObject gameController;
    private PauseController pauseController;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerScale = gameObject.transform.localScale;
        grabScript = GetComponent<Grab>();
        climb = GetComponent<Climb>();  
        mantle = GetComponent<Mantle>();
        gameController = GameObject.FindWithTag("GameController");
        pauseController = gameController.GetComponent<PauseController>();
    }
    void Update()
    {
        // Movement animation logic and param
        if (moveInput.magnitude > 0)  {
            animator.SetBool("Move", true);
            animator.SetFloat("Move Speed Multiplyer", moveSpeedMultiplier * moveInput.x);

            if (interactable == null || interactable.GetType() != typeof(Grab)) 
            {
                if (moveInput.x > 0)
                {
                    gameObject.transform.localScale = new Vector3(playerScale.x, playerScale.y, playerScale.z);
                }
                else if (moveInput.x < 0)
                {
                    gameObject.transform.localScale = new Vector3(-playerScale.x, playerScale.y, playerScale.z);
                }
            }
        }
        else {
            animator.SetBool("Move", false);
        }
        // Set jump and fall animation param
        animator.SetFloat("Vertical Velocity", rb.velocity.y);
        animator.SetFloat("Horizontal Velocity", rb.velocity.x);

        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer) && !mantle.MantleEnabled;
        if (isGrounded)   {
            jumpYCoordinate = transform.position.y;
            animator.SetBool("Grounded", true);
            coyoteTimeCounter = coyoteTime;
            if (fallingSpeed <= -fallingMaxSpeed) 
            {
                GetComponent<Death>().TriggerDeath();
            }
        }
        else {
            animator.SetBool("Grounded", false);
            coyoteTimeCounter -= Time.deltaTime;
            fallingSpeed = rb.velocity.y;
        }

        // Jump logic
        if (jumpPressed && coyoteTimeCounter > 0f && !mantle.MantleEnabled)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        jumpPressed = false; // Reset jump flag
    }

    void FixedUpdate()
    {
        velocityX = Mathf.Lerp(velocityX, moveInput.x * moveSpeed, Time.deltaTime * 8f);
        // Horizontal movement
        if (!climb.Climbing()) 
        {
            rb.velocity = new Vector2(velocityX, rb.velocity.y);
        }
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

    public void OnPause()
    {
        Debug.Log("from player pause pressed");
        pauseController.OnPause();
    }

    public bool CheckGrounded() => isGrounded;

    public void SetMoveSpeedMultiplier(float m) 
    { 
        moveSpeedMultiplier = m;
    }
    public void ResetMoveSpeed() 
    { 
        moveSpeedMultiplier = 1f;
    }

    public void SetInteraction(Interactable interaction) 
    {
        interactable = interaction;
    }

    public bool Interacting() 
    {
        if (interactable == null) 
        {
            return false;
        }
        return true;
    }

    public float GetLastYCoordinate() 
    {
        return jumpYCoordinate;
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
