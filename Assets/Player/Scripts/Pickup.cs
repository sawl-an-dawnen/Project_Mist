using UnityEngine;

public class Pickup : Interactable
{
    [Header("Pickup Settings")]
    public float weight = 1f;

    [Header("Holding Physics")]
    public float holdingForce = 250f;
    public float holdingFrequency = 8f;

    [Range(0f, 1f)]
    public float holdingDamping = 0.9f;

    [Header("Automatic Drop")]
    public bool dropWhenPushingDown = true;
    public float downwardObstructionThreshold = 0.12f;
    public float supportNormalThreshold = 0.5f;

    [Header("Inverted Gravity")]
    public float liftStrengthPerWeight = 1f;
    public float minimumPlayerGravityMultiplier = 0.4f;

    private bool held = false;
    private bool wasInvertedWhileHeld = false;

    private Transform rightArmBoneHolding;
    private Transform leftArmBoneHolding;

    private GameObject[] animationArmSprites;
    private GameObject[] holdingArmSprites;

    private float playerOriginalGravity;
    private float originalGravityMagnitude;
    private int layerState;

    private RigidbodyType2D originalBodyType;
    private bool originalSimulated;

    private CollisionDetectionMode2D originalCollisionDetection;

    private Rigidbody2D grabbedObject;
    private Collider2D grabbedCollider;

    private Rigidbody2D player;
    private Transform holdPoint;

    private Movement moveScript;
    private Interact interactor;

    private TargetJoint2D holdingJoint;

    private void Awake()
    {
        oneShot = false;

        grabbedObject = GetComponent<Rigidbody2D>();
        grabbedCollider = GetComponent<Collider2D>();

        layerState = gameObject.layer;

        originalBodyType = grabbedObject.bodyType;
        originalSimulated = grabbedObject.simulated;

        originalCollisionDetection =
            grabbedObject.collisionDetectionMode;

        // Save the object's original gravity strength
        originalGravityMagnitude =
            Mathf.Abs(grabbedObject.gravityScale);

        player = GameObject.FindGameObjectWithTag("Player")
            .GetComponent<Rigidbody2D>();

        moveScript = player.GetComponent<Movement>();
        interactor = player.GetComponent<Interact>();

        holdPoint = GameObject.FindGameObjectWithTag(
            "HoldPoint").transform;

        rightArmBoneHolding = GameObject.FindGameObjectWithTag(
            "Left_arm_bone").transform;

        leftArmBoneHolding = GameObject.FindGameObjectWithTag(
            "Right_arm_bone").transform;

        animationArmSprites =
            GameObject.FindGameObjectsWithTag("Arms_animation");

        holdingArmSprites =
            GameObject.FindGameObjectsWithTag("Arms_holding");
    }

    private void Update()
    {
        if (!held)
            return;

        if (holdPoint == null)
        {
            Release();

            if (interactor != null)
                interactor.CancelInteraction();

            return;
        }
    }

    private void FixedUpdate()
    {
        if (!held || holdingJoint == null || holdPoint == null)
            return;

        // Follow the player's hold point using physics
        holdingJoint.target = holdPoint.position;

        // Check whether gravity has changed while carrying
        bool currentlyInverted =
            grabbedObject.gravityScale < 0f;

        if (currentlyInverted != wasInvertedWhileHeld)
        {
            wasInvertedWhileHeld = currentlyInverted;
            UpdatePlayerGravity();
        }
    }

    private void LateUpdate()
    {
        if (!held)
            return;

        if (rightArmBoneHolding == null ||
            leftArmBoneHolding == null ||
            player == null)
            return;

        Transform target = grabbedObject.transform;

        Vector2 direction =
            target.position - rightArmBoneHolding.position;

        float facingMultiplier =
            transform.localScale.x >= 0 ? 1f : -1f;

        bool isFacingRight =
            player.transform.localScale.x >= 0;

        float angle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (!isFacingRight)
            angle = 180f - angle;

        angle *= facingMultiplier;

        rightArmBoneHolding.rotation =
            Quaternion.Euler(0, 0, angle);

        leftArmBoneHolding.rotation =
            Quaternion.Euler(0, 0, angle);

        Debug.DrawLine(
            rightArmBoneHolding.position,
            target.position,
            Color.red
        );

        Debug.DrawLine(
            leftArmBoneHolding.position,
            target.position,
            Color.green
        );

        if (grabbedCollider != null &&
            !grabbedCollider.isActiveAndEnabled)
        {
            Release();

            if (interactor != null)
                interactor.CancelInteraction();
        }
    }

    public override void Interact()
    {
        if (held || holdPoint == null)
            return;

        held = true;

        ToggleArmVisibility();

        // Save object physics settings
        originalBodyType = grabbedObject.bodyType;
        originalSimulated = grabbedObject.simulated;

        originalCollisionDetection =
            grabbedObject.collisionDetectionMode;

        layerState = gameObject.layer;

        // Save the player's current gravity
        playerOriginalGravity = player.gravityScale;

        // Refresh the gravity magnitude if valid
        if (!Mathf.Approximately(grabbedObject.gravityScale, 0f))
        {
            originalGravityMagnitude =
                Mathf.Abs(grabbedObject.gravityScale);
        }

        // Keep physics enabled while carried
        grabbedObject.simulated = true;
        grabbedObject.bodyType = RigidbodyType2D.Dynamic;

        grabbedObject.collisionDetectionMode =
            CollisionDetectionMode2D.Continuous;

        grabbedObject.velocity = Vector2.zero;
        grabbedObject.angularVelocity = 0f;

        // Do not disable gravity while carrying.
        // This allows inversion to work normally.

        int grabbedLayer =
            LayerMask.NameToLayer("Grabbed Layer");

        if (grabbedLayer >= 0)
            gameObject.layer = grabbedLayer;

        // Create physics-based holding joint
        holdingJoint =
            gameObject.AddComponent<TargetJoint2D>();

        holdingJoint.autoConfigureTarget = false;
        holdingJoint.anchor = grabbedObject.centerOfMass;
        holdingJoint.target = holdPoint.position;

        holdingJoint.maxForce = holdingForce;
        holdingJoint.frequency = holdingFrequency;
        holdingJoint.dampingRatio = holdingDamping;

        // Apply movement penalty based on weight
        if (moveScript != null)
        {
            moveScript.SetMoveSpeedMultiplier(
                1f / Mathf.Max(weight, 0.01f)
            );
        }

        // Apply inverted gravity effect
        wasInvertedWhileHeld =
            grabbedObject.gravityScale < 0f;

        UpdatePlayerGravity();

        Debug.Log("[Pickup] Object picked up: " + name);
    }

    private void UpdatePlayerGravity()
    {
        if (!held || player == null)
            return;

        if (grabbedObject.gravityScale < 0f)
        {
            float gravityMultiplier = Mathf.Clamp(
                1f - (weight * liftStrengthPerWeight),
                minimumPlayerGravityMultiplier,
                1f
            );

            player.gravityScale =
                playerOriginalGravity * gravityMultiplier;

            Debug.Log(
                "[Pickup] Inverted gravity. Player gravity: " +
                player.gravityScale
            );
        }
        else
        {
            // Restore normal player gravity
            player.gravityScale = playerOriginalGravity;
        }
    }

    public override void Release()
    {
        if (!held)
            return;

        held = false;

        // Remove holding forces immediately
        if (holdingJoint != null)
        {
            holdingJoint.enabled = false;
            Destroy(holdingJoint);
            holdingJoint = null;
        }

        if (holdPoint != null)
            ToggleArmVisibility();

        // Restore original layer
        gameObject.layer = layerState;

        // Restore original physics settings
        grabbedObject.bodyType = originalBodyType;
        grabbedObject.simulated = originalSimulated;

        grabbedObject.collisionDetectionMode =
            originalCollisionDetection;

        // Preserve inverted gravity if negative.
        // Restore positive gravity if it accidentally became zero.
        if (Mathf.Approximately(grabbedObject.gravityScale, 0f))
        {
            grabbedObject.gravityScale =
                originalGravityMagnitude;

            Debug.Log(
                "[Pickup] Restored object gravity to: " +
                grabbedObject.gravityScale
            );
        }

        // Restore player gravity
        if (player != null)
            player.gravityScale = playerOriginalGravity;

        wasInvertedWhileHeld = false;

        if (moveScript != null)
            moveScript.ResetMoveSpeed();

        Debug.Log(
            "[Pickup] Object released. Gravity: " +
            grabbedObject.gravityScale
        );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckDownwardObstruction(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        CheckDownwardObstruction(collision);
    }

    private void CheckDownwardObstruction(
        Collision2D collision)
    {
        if (!held ||
            !dropWhenPushingDown ||
            holdPoint == null)
            return;

        if (player != null &&
            collision.gameObject == player.gameObject)
            return;

        // Check whether the hold point is trying to
        // pull the object downward through another object.
        float downwardCompression =
            grabbedObject.position.y - holdPoint.position.y;

        if (downwardCompression <
            downwardObstructionThreshold)
            return;

        // Check whether the object is supported from below
        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint2D contact =
                collision.GetContact(i);

            if (contact.normal.y > supportNormalThreshold)
            {
                Debug.Log(
                    "[Pickup] Downward obstruction detected. " +
                    "Dropping object: " + name
                );

                Release();

                if (interactor != null)
                    interactor.CancelInteraction();

                return;
            }
        }
    }

    public bool IsHeld()
    {
        return held;
    }

    private void ToggleArmVisibility()
    {
        foreach (GameObject arm in animationArmSprites)
        {
            if (arm == null)
                continue;

            SpriteRenderer sprite =
                arm.GetComponent<SpriteRenderer>();

            if (sprite != null)
                sprite.enabled = !sprite.enabled;
        }

        foreach (GameObject arm in holdingArmSprites)
        {
            if (arm == null)
                continue;

            SpriteRenderer sprite =
                arm.GetComponent<SpriteRenderer>();

            if (sprite != null)
                sprite.enabled = !sprite.enabled;
        }
    }
}