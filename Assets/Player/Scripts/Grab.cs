using UnityEngine;
using UnityEngine.Profiling;

public class Grab : Interactable
{
    public float damping = 2f; // Resistance for smoother dragging
    public float frequency = 5f; // Frequency for spring-like effect
    public float weight = 1f;
    public float maxAngle = 50f;
    public float minAngle = -40f;
    public float maxHoldVelocity = 2.5f;

    private bool held = false;
    private Transform holdPoint; // A point near the character where the object will be held

    private Transform rightArmBoneHolding; // The main bone for the arm
    private Transform leftArmBoneHolding; // The main bone for the arm

    private GameObject[] animationArmSprites;
    private GameObject[] holdingArmSprites;

    private Rigidbody2D grabbedObject; // Currently grabbed object's Rigidbody2D
    private Collider2D grabbedCollider; // Currently grabbed object's Collider2D
    private float gravityScaleDefaultValue;
    private LayerMask layerStateDefaultValue;
    private Rigidbody2D player;
    private Movement moveScript;
    private Interact interactor;
    private HingeJoint2D joint;

    private void Awake()
    {
        oneShot = false;
        grabbedObject = GetComponent<Rigidbody2D>();
        grabbedCollider = GetComponent<Collider2D>();   
        gravityScaleDefaultValue = grabbedObject.gravityScale;
        layerStateDefaultValue = grabbedObject.gameObject.layer;

        holdPoint = GameObject.FindGameObjectWithTag("HoldPoint").transform;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        moveScript = player.gameObject.GetComponent<Movement>();
        interactor = player.gameObject.GetComponent<Interact>();

        rightArmBoneHolding = GameObject.FindGameObjectWithTag("Left_arm_bone").transform;
        leftArmBoneHolding = GameObject.FindGameObjectWithTag("Right_arm_bone").transform;
        animationArmSprites = GameObject.FindGameObjectsWithTag("Arms_animation");
        holdingArmSprites = GameObject.FindGameObjectsWithTag("Arms_holding");
    }

    private void LateUpdate()
    {
        if (held)
        {
            Transform target = grabbedObject.gameObject.transform;
            // Calculate direction from arm bone to the object
            if (rightArmBoneHolding != null) {
                Vector2 direction = target.position - rightArmBoneHolding.position;
                float facingMultiplier = player.gameObject.transform.localScale.x >= 0 ? 1 : -1; // Check if facing right (positive scale) or left (negative scale)
                bool isFacingRight = player.gameObject.transform.localScale.x >= 0;

                // Convert direction to an angle
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                if (!isFacingRight)
                {
                    angle = 180 - angle; // Adjust the angle for flipping
                }
                angle *= facingMultiplier;

                // Apply rotation to the arm bone
                rightArmBoneHolding.rotation = Quaternion.Euler(0, 0, angle);
                leftArmBoneHolding.rotation = Quaternion.Euler(0, 0, angle);

                Debug.DrawLine(rightArmBoneHolding.position, target.position, Color.red);  // First arm to target
                Debug.DrawLine(leftArmBoneHolding.position, target.position, Color.green); // Second arm to target
                if (!grabbedCollider.isActiveAndEnabled)
                {
                    Release();
                    interactor.CancelInteraction();
                }
            }
            if (player.velocity.magnitude >= maxHoldVelocity) 
            {
                Release();
                interactor.CancelInteraction();
            }
            //If the gravity scale of the object has been changed (by another script) while being held, update the stored gravity scale so it can be reset properly on release
            if (grabbedObject.gravityScale != 0f && grabbedObject.gravityScale != gravityScaleDefaultValue)
            {
                gravityScaleDefaultValue = grabbedObject.gravityScale;
            }
        }
    }

    public override void Interact()
    {
        held = true;
        ToggleArmVisibility();
        if (grabbedObject.gravityScale >= 0f)
        {
            grabbedObject.gravityScale = 1f;
        }
        grabbedObject.gameObject.layer = LayerMask.NameToLayer("Grabbed Layer");
        moveScript.SetMoveSpeedMultiplier(1 / weight);
        moveScript.SetInteraction(GetComponent<Grab>());
        CreateJoint(grabbedObject.ClosestPoint(player.gameObject.transform.position));
    }

    public override void Release()
    {
        held = false;
        ToggleArmVisibility();
        grabbedObject.gameObject.layer = layerStateDefaultValue; // Reset layer
        grabbedObject.gravityScale = gravityScaleDefaultValue; // Restore gravity
        moveScript.ResetMoveSpeed();
        moveScript.SetInteraction(null);
        Destroy(joint);
    }

    void ToggleArmVisibility()
    {
        foreach (GameObject arm in animationArmSprites)
        {
            SpriteRenderer sprite = arm.GetComponent<SpriteRenderer>();
            sprite.enabled = !sprite.enabled;
        }
        foreach (GameObject arm in holdingArmSprites)
        {
            SpriteRenderer sprite = arm.GetComponent<SpriteRenderer>();
            sprite.enabled = !sprite.enabled;
        }
    }

    void CreateJoint(Vector2 latchPoint)
    {
        if (grabbedObject == null) return;

        // Add a Distance Joint to the player
        joint = player.gameObject.AddComponent<HingeJoint2D>();
        joint.connectedBody = grabbedObject;

        // Set the joint anchor to the player's position
        joint.anchor = holdPoint.transform.InverseTransformPoint(holdPoint.transform.position);

        // Set the connected anchor to the latch point
        joint.connectedAnchor = grabbedObject.transform.InverseTransformPoint(latchPoint);

        // Configure angle limits
        JointAngleLimits2D limits = new JointAngleLimits2D();
        limits.min = minAngle; // Limit rotation to -30 degrees
        limits.max = maxAngle;  // Limit rotation to +30 degrees
        joint.limits = limits;
        joint.useLimits = true;
    }
}
