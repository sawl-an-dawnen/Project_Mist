using UnityEngine;

public class Mantle : MonoBehaviour
{
    //public float heightOffset = 0.1f;
    //public float mantleHeight = .2f;   // Height above the player to detect a ledge
    public float mantleDistance = 0.5f; // Distance in front of the player to detect a ledge
    public float climbHeight = .2f;
    public float forwardMove = .2f;
    public float mantleSpeed = 5f;     // Speed of the mantling movement
    public LayerMask ledgeLayer;       // Layer for ledges

    private Rigidbody2D rb;
    private bool isMantling = false;
    private Vector2 mantleTarget;
    private Vector2 upwardRayOrigin;
    private Movement moveScript;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        moveScript = GetComponent<Movement>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (moveScript.checkGrounded() && !isMantling && Input.GetKey(KeyCode.Space)) // Check for mantle input (Space)
        {
            Debug.Log("Check for ledge");
            CheckForLedge();
        }
    }

    private void CheckForLedge()
    {
        // Cast a ray forward to detect the ledge
        //Vector2 rayOrigin = new Vector2(transform.position.x + (.1f*Mathf.Sign(transform.localScale.x)), transform.position.y + heightOffset); // Start ray slightly above ground
        Vector2 rayOrigin = new Vector2(transform.position.x + (.1f * Mathf.Sign(transform.localScale.x)), transform.position.y); // Start ray slightly above ground
        Vector2 rayDirection = transform.right * Mathf.Sign(transform.localScale.x); // Ray direction depends on facing direction

        Debug.DrawRay(rayOrigin, rayDirection * (mantleDistance+.1f), Color.red);

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, mantleDistance, ledgeLayer);
        if (hit.collider != null)
        {
            Debug.Log("Climbable object detected.");
            // Cast another ray upward from the ledge to find the top
            upwardRayOrigin = hit.point + Vector2.up * 0.1f;
            RaycastHit2D upwardHit = Physics2D.Raycast(upwardRayOrigin, Vector2.up, 0f, ledgeLayer);

            if (upwardHit.collider == null) // No collider above means it's climbable
            {
                StartMantle(hit.point);
            }
        }
    }

    private void StartMantle(Vector2 ledgePoint)
    {
        Debug.Log("Target Set.");
        isMantling = true;
        animator.SetBool("Mantle", true);

        // Calculate target position for mantling
        mantleTarget = new Vector2(ledgePoint.x + (forwardMove * Mathf.Sign(transform.localScale.x)), ledgePoint.y + climbHeight); // Adjust height as needed
        Debug.Log(mantleTarget);
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero; // Stop player movement during mantling
    }

    private void FixedUpdate()
    {
        if (isMantling)
        {
            Debug.Log("Manteling");
            // Smoothly move the player to the mantle target
            transform.position = Vector2.MoveTowards(transform.position, mantleTarget, mantleSpeed * Time.fixedDeltaTime);
            RaycastHit2D upwardHit = Physics2D.Raycast(mantleTarget, Vector2.up, 0f, ledgeLayer);

            // Stop mantling when the target is reached
            if (Vector2.Distance(transform.position, mantleTarget) < 0.1f || upwardHit.collider != null)
            {
                isMantling = false;
                animator.SetBool("Mantle", false);
                rb.gravityScale = 1f;
            }
        }
    }

    void OnDrawGizmos()
    {
        // Visualize grab range in the editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(mantleTarget, 0.05f);
        Gizmos.DrawSphere(upwardRayOrigin, 0.05f);

    }
}