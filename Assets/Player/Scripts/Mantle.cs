using UnityEngine;

public class Mantle : MonoBehaviour
{
    //public float heightOffset = 0.1f;
    //public float mantleHeight = .2f;   // Height above the player to detect a ledge
    public float mantleDistance = 0.5f; // Distance in front of the player to detect a ledge
    public float climbHeight = .2f; // How high the player can jump up
    public float forwardMove = .2f;
    public float mantleSpeed = 5f;     // Speed of the mantling movement
    public LayerMask ledgeLayer;       // Layer for ledges

    private Rigidbody2D rb;
    private bool isMantling = false;
    private float currentScale;
    private Vector2 mantleTarget;
    private Vector2 upwardRayOrigin;
    private Movement moveScript;
    private Grab grabScript;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        moveScript = GetComponent<Movement>();
        grabScript = GetComponent<Grab>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isMantling && moveScript.CheckGrounded() && !grabScript.HoldingObject() && Input.GetKey(KeyCode.Space)) // Check for mantle input (Space)
        {
            CheckForLedge();
        }
    }

    private void CheckForLedge()
    {
        // Cast a ray forward to detect the ledge
        Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y);
        Vector2 rayDirection = transform.right * Mathf.Sign(transform.localScale.x); // Ray direction depends on facing direction

        Debug.DrawRay(rayOrigin, rayDirection * (mantleDistance), Color.red);

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, mantleDistance, ledgeLayer);
        if (hit.collider != null)
        {
            // Cast another ray upward from the ledge to find the top
            upwardRayOrigin = hit.point + Vector2.up * climbHeight + (Vector2.right * .05f * Mathf.Sign(transform.localScale.x));
            RaycastHit2D upwardHit = Physics2D.Raycast(upwardRayOrigin, Vector2.up, 0f, ledgeLayer);

            //Debug.Log(upwardHit.collider.name);

            if (upwardHit.collider == null && moveScript.CheckGrounded()) // No collider above means it's climbable
            {
                StartMantle(hit.point);
            }
        }
    }

    private void StartMantle(Vector2 ledgePoint)
    {
        animator.SetBool("Mantle", true);
        // Calculate target position for mantling
        mantleTarget = new Vector2(ledgePoint.x + (forwardMove * Mathf.Sign(transform.localScale.x)), ledgePoint.y + climbHeight); // Adjust height as needed
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero; // Stop player movement during mantling

        currentScale = Mathf.Sign(transform.localScale.x);

        isMantling = true;
    }

    private void FixedUpdate()
    {
        if (isMantling)
        {
            // Smoothly move the player to the mantle target
            transform.position = Vector2.MoveTowards(transform.position, mantleTarget, mantleSpeed * Time.fixedDeltaTime);
            RaycastHit2D mantleHit = Physics2D.Raycast(mantleTarget, Vector2.up, 0f, ledgeLayer);

            // Stop mantling when the target is reached
            if (Vector2.Distance(transform.position, mantleTarget) < 0.1f || mantleHit.collider != null || Mathf.Sign(transform.localScale.x) != currentScale)
            {
                isMantling = false;
                animator.SetBool("Mantle", false);
                rb.gravityScale = 1f;
            }
        }
    }

    public bool MantleEnabled { get { return isMantling; } }

    void OnDrawGizmos()
    {
        // Visualize grab range in the editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(mantleTarget, 0.05f);
        Gizmos.DrawSphere(upwardRayOrigin, 0.05f);

    }
}