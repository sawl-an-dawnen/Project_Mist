using System.Collections;
using UnityEngine;

public class Mantle : MonoBehaviour
{
    [Header("Mantle Settings")]
    public float distanceToLedge = 0.5f;
    public float climbableHeight = 0.17f;
    public float upwardDistance = 0.25f;
    public float forwardDistance = 0.2f;
    public float mantleSpeed = 1.3f;
    public float mantleDuration = 0.6f;

    private Rigidbody2D rb;
    private Rigidbody2D objectRigidbody;

    private RigidbodyType2D objectBodyType;

    private bool isMantling = false;

    private float currentScale;
    private float startingGravityScale;

    private Vector2 mantleTarget;
    private Vector2 upwardRayOrigin;

    private LayerMask climbableLayer;

    private Movement moveScript;
    private Grab grabScript;
    private Climb climbScript;
    private Animator animator;
    private Interact interactor;
    private GameManager gameManager;

    private Coroutine mantleCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        moveScript = GetComponent<Movement>();
        grabScript = GetComponent<Grab>();
        climbScript = GetComponent<Climb>();

        animator = GetComponent<Animator>();
        interactor = GetComponent<Interact>();

        climbableLayer = LayerMask.GetMask("Climbable");

        gameManager = GameManager.Instance;
    }

    private void FixedUpdate()
    {
        if (!isMantling)
            return;

        // Smoothly move the player toward the mantle target
        rb.MovePosition(
            Vector2.MoveTowards(
                rb.position,
                mantleTarget,
                mantleSpeed * Time.fixedDeltaTime
            )
        );

        // Check whether the player reached the target
        bool reachedTarget =
            Vector2.Distance(rb.position, mantleTarget) < 0.1f;

        // Check whether the player changed facing direction
        bool changedDirection =
            Mathf.Sign(transform.localScale.x) != currentScale;

        if (reachedTarget || changedDirection)
        {
            FinishMantle();
        }
    }

    private void CheckForLedge()
    {
        Vector2 rayOrigin = transform.position;

        Vector2 rayDirection =
            transform.right *
            Mathf.Sign(transform.localScale.x);

        RaycastHit2D hit = Physics2D.Raycast(
            rayOrigin,
            rayDirection,
            distanceToLedge,
            climbableLayer
        );

        if (hit.collider == null)
            return;

        upwardRayOrigin =
            hit.point +
            Vector2.up * climbableHeight +
            Vector2.right * 0.05f *
            Mathf.Sign(transform.localScale.x);

        RaycastHit2D upwardHit = Physics2D.Raycast(
            upwardRayOrigin,
            Vector2.up,
            0f,
            climbableLayer
        );

        if (upwardHit.collider == null &&
            transform.position.y <=
            moveScript.GetLastYCoordinate() + 0.03f)
        {
            StartMantle(hit);
        }
    }

    private void StartMantle(RaycastHit2D hit)
    {
        if (isMantling)
            return;

        // IMPORTANT:
        // End climbing before saving mantle gravity.
        if (climbScript != null && climbScript.Climbing())
        {
            climbScript.StopClimbingForMantle();
        }

        // Gravity is now restored by Climb.
        startingGravityScale = rb.gravityScale;

        objectRigidbody = hit.collider.attachedRigidbody;

        if (objectRigidbody != null)
        {
            objectBodyType = objectRigidbody.bodyType;

            objectRigidbody.bodyType =
                RigidbodyType2D.Kinematic;

            objectRigidbody.velocity = Vector2.zero;
        }

        Vector2 ledgePoint = hit.point;

        mantleTarget = new Vector2(
            ledgePoint.x +
            forwardDistance * Mathf.Sign(transform.localScale.x),
            ledgePoint.y + upwardDistance
        );

        currentScale = Mathf.Sign(transform.localScale.x);

        isMantling = true;

        // Disable gravity for mantle movement
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;

        animator.SetBool("Mantle", true);
        animator.Play("Player_mantle", 0, 0f);

        mantleCoroutine = StartCoroutine(MantleTimeout());
    }

    private IEnumerator MantleTimeout()
    {
        yield return new WaitForSeconds(mantleDuration);

        if (isMantling)
        {
            FinishMantle();
        }
    }

    private void FinishMantle()
    {
        if (!isMantling)
            return;

        isMantling = false;

        // Stop the timeout coroutine if mantle finished early
        if (mantleCoroutine != null)
        {
            StopCoroutine(mantleCoroutine);
            mantleCoroutine = null;
        }

        // Restore ledge object's physics
        if (objectRigidbody != null)
        {
            objectRigidbody.bodyType = objectBodyType;
            objectRigidbody = null;
        }

        // Restore player gravity
        rb.gravityScale = startingGravityScale;

        animator.SetBool("Mantle", false);

        Debug.Log(
            "[Mantle] Finished. Restored gravity: " +
            rb.gravityScale
        );
    }

    private void OnDisable()
    {
        if (isMantling)
        {
            FinishMantle();
        }
    }

    public bool MantleEnabled
    {
        get { return isMantling; }
    }

    public void OnMantle()
    {
        if (!isMantling &&
            !interactor.Interacting() &&
            gameManager.InControl() &&
            !gameManager.Paused())
        {
            CheckForLedge();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawSphere(mantleTarget, 0.05f);
        Gizmos.DrawSphere(upwardRayOrigin, 0.05f);
    }
}