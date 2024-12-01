using UnityEngine;
using UnityEngine.Profiling;

public class IGrab : Interactable
{
    public float damping = 2f; // Resistance for smoother dragging
    public float frequency = 5f; // Frequency for spring-like effect

    private bool held = false;

    private Transform rightArmBoneHolding; // The main bone for the arm
    private Transform leftArmBoneHolding; // The main bone for the arm
    public Transform grabPoint;
    public Transform holdPoint; // A point near the character where the object will be held
    public GameObject[] animationArmSprites;
    public GameObject[] holdingArmSprites;

    public float grabRange = 1f; // Maximum distance to grab objects
    public LayerMask grabbableLayer; // Layer for grabbable objects
    private Rigidbody2D grabbedObject; // Currently grabbed object's Rigidbody2D
    private float objGravityScale;
    private LayerMask layerState;
    private Movement moveScript;
    private SpringJoint2D joint;
    private Rigidbody2D player;

    private void Awake()
    {

        moveScript = GetComponent<Movement>();

        oneShot = false;
        grabbedObject = GetComponent<Rigidbody2D>();
        objGravityScale = grabbedObject.gravityScale;
        layerState = grabbedObject.gameObject.layer;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        moveScript = player.gameObject.GetComponent<Movement>();
        holdPoint = GameObject.FindGameObjectWithTag("HoldPoint").transform;

        rightArmBoneHolding = GameObject.FindGameObjectWithTag("Left_arm_bone").transform;
        leftArmBoneHolding = GameObject.FindGameObjectWithTag("Right_arm_bone").transform;

        animationArmSprites = GameObject.FindGameObjectsWithTag("Arms_animation");
        holdingArmSprites = GameObject.FindGameObjectsWithTag("Arms_holding");
    }

    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (held)
        {
            Transform target = grabbedObject.gameObject.transform;
            // Calculate direction from arm bone to the object
            Vector2 direction = target.position - rightArmBoneHolding.position;
            float facingMultiplier = gameObject.transform.localScale.x >= 0 ? 1 : -1; // Check if facing right (positive scale) or left (negative scale)
            bool isFacingRight = gameObject.transform.localScale.x >= 0;

            // Convert direction to an angle
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (!isFacingRight)
            {
                angle = 180 - angle; // Adjust the angle for flipping
            }
            angle *= facingMultiplier;
            //Debug.Log(angle);

            // Apply rotation to the arm bone
            rightArmBoneHolding.rotation = Quaternion.Euler(0, 0, angle);
            leftArmBoneHolding.rotation = Quaternion.Euler(0, 0, angle);

            Debug.DrawLine(rightArmBoneHolding.position, target.position, Color.red);  // First arm to target
            Debug.DrawLine(leftArmBoneHolding.position, target.position, Color.green); // Second arm to target
        }
    }

    public override void Interact()
    {
        held = true;
        ToggleArmVisibility();
        grabbedObject = GetComponent<Rigidbody2D>();
        objGravityScale = grabbedObject.gravityScale;
        grabbedObject.gravityScale = 1f;
        layerState = grabbedObject.gameObject.layer;
        grabbedObject.gameObject.layer = LayerMask.NameToLayer("Grabbed Layer");
        moveScript.SetMoveSpeedMultiplier(1 / grabbedObject.mass);
        CreateJoint(grabbedObject.ClosestPoint(player.gameObject.transform.position));
    }

    public override void Release()
    {
        held = false;

        ToggleArmVisibility();
        grabbedObject.gameObject.layer = layerState; // Reset layer
        grabbedObject.gravityScale = objGravityScale; // Restore gravity
        Destroy(joint);
        grabbedObject = null;
        moveScript.ResetMoveSpeed();
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

    public bool HoldingObject()
    {
        return held;
    }

    void CreateJoint(Vector2 latchPoint)
    {
        if (grabbedObject == null) return;

        // Add a Distance Joint to the player
        joint = player.gameObject.AddComponent<SpringJoint2D>();
        joint.connectedBody = grabbedObject;

        // Set the joint anchor to the player's position
        joint.anchor = grabPoint.transform.InverseTransformPoint(grabPoint.transform.position);

        // Set the connected anchor to the latch point
        joint.connectedAnchor = grabbedObject.transform.InverseTransformPoint(latchPoint);

        // Configure the joint for smooth dragging
        joint.autoConfigureDistance = true;
        joint.distance = .05f; // No distance between the player and the latch point
        joint.dampingRatio = damping; // Smooth movement
        joint.frequency = frequency; // Spring-like effect
    }

    void OnDrawGizmos()
    {
        // Visualize grab range in the editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(grabPoint.position, grabRange);
        //Gizmos.DrawWireSphere(holdPoint.position, grabRange);
    }
}
