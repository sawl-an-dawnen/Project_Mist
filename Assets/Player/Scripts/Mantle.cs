using System.Collections;
using UnityEngine;

public class Mantle : MonoBehaviour
{
    public float distanceToLedge = 0.5f; // Distance in front of the player to detect a ledge
    public float climbHeight = .2f; // How high the player can jump up
    public float forwardDistance = .2f;
    public float mantleSpeed = 5f;     // Speed of the mantling movement

    
    private Rigidbody2D rb;
    private Rigidbody2D objectRigidbody;
    RigidbodyType2D objectBodyType;
    private bool isMantling = false;
    private float currentScale;
    private Vector2 mantleTarget;
    private Vector2 upwardRayOrigin;
    private LayerMask climbableLayer; // Layer for ledges
    private Movement moveScript;
    private Grab grabScript;
    private Animator animator;
    private Interact interactor;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        moveScript = GetComponent<Movement>();
        grabScript = GetComponent<Grab>();
        animator = GetComponent<Animator>();
        interactor = GetComponent<Interact>();
        climbableLayer = LayerMask.GetMask("Climbable");
    }

    private void Update()
    {
        if (!isMantling && !interactor.Interacting() && Input.GetKey(KeyCode.Space)) // Check for mantle input (Space)
        {
            CheckForLedge();
        }
    }

    private void CheckForLedge()
    {
        // Cast a ray forward to detect the ledge
        Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y);
        Vector2 rayDirection = transform.right * Mathf.Sign(transform.localScale.x); // Ray direction depends on facing direction

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, distanceToLedge, climbableLayer);
        if (hit.collider != null)
        {
            // Cast another ray upward from the ledge to find the top
            upwardRayOrigin = hit.point + Vector2.up * climbHeight + (Vector2.right * .05f * Mathf.Sign(transform.localScale.x));
            RaycastHit2D upwardHit = Physics2D.Raycast(upwardRayOrigin, Vector2.up, 0f, climbableLayer);

            //Debug.Log(upwardHit.collider.name);

            if (upwardHit.collider == null && (transform.position.y <= moveScript.GetLastYCoordinate() + .03f)) // No collider above means it's climbable, can mantle at the end of jump but only at a reduced or level height
            {
                StartMantle(hit);
            }
        }
    }

    private void StartMantle(RaycastHit2D hit)
    {
        Vector2 ledgePoint = hit.point;
        objectRigidbody = hit.collider.attachedRigidbody;
        // Disable collision between the player and the object
        //Physics2D.IgnoreCollision(rb.gameObject.GetComponent<Collider2D>(), objectRigidbody.GetComponent<Collider2D>(), true);

        StartCoroutine(CompleteMantle(ledgePoint));
    }

    IEnumerator CompleteMantle(Vector2 ledgePoint)
    {
        // Mantle logic here (move the player to the mantle target)
        objectBodyType = objectRigidbody.bodyType;
        animator.SetBool("Mantle", true);
        objectRigidbody.bodyType = RigidbodyType2D.Kinematic;
        objectRigidbody.velocity = Vector2.zero;

        // Calculate target position for mantling
        mantleTarget = new Vector2(ledgePoint.x + (forwardDistance * Mathf.Sign(transform.localScale.x)), ledgePoint.y + climbHeight); // Adjust height as needed
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero; // Stop player movement during mantling
        currentScale = Mathf.Sign(transform.localScale.x);
        isMantling = true;
        // Smoothly move the player to the mantle target
        transform.position = Vector2.MoveTowards(transform.position, mantleTarget, mantleSpeed * Time.fixedDeltaTime);
        RaycastHit2D mantleHit = Physics2D.Raycast(mantleTarget, Vector2.up, 0f, climbableLayer);

        yield return new WaitForSeconds(0.6f); // Adjust based on mantle duration

        // Re-enable collision after mantle is complete
        objectRigidbody.bodyType = objectBodyType;
        isMantling = false;
        animator.SetBool("Mantle", false);
        rb.gravityScale = 1f;
        //Physics2D.IgnoreCollision(rb.gameObject.GetComponent<Collider2D>(), objectRigidbody.GetComponent<Collider2D>(), false);
    }

    private void FixedUpdate()
    {
        if (isMantling)
        {
            // Smoothly move the player to the mantle target
            transform.position = Vector2.MoveTowards(transform.position, mantleTarget, mantleSpeed * Time.fixedDeltaTime);
            RaycastHit2D mantleHit = Physics2D.Raycast(mantleTarget, Vector2.up, 0f, climbableLayer);

            // Stop mantling when the target is reached
            if (Vector2.Distance(transform.position, mantleTarget) < 0.1f || mantleHit.collider != null || Mathf.Sign(transform.localScale.x) != currentScale)
            {
                objectRigidbody.bodyType = objectBodyType;
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