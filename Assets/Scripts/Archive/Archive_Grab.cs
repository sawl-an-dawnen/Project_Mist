using UnityEngine;
using UnityEngine.Profiling;

public class Archive_Grab : MonoBehaviour
{
    public float damping = 2f; // Resistance for smoother dragging
    public float frequency = 5f; // Frequency for spring-like effect

    public Transform armBone01; // The main bone for the arm
    public Transform armBone02; // The main bone for the arm
    public Transform grabPoint;
    public Transform holdPoint; // A point near the character where the object will be held
    public GameObject[] animationArms;
    public GameObject[] grabArms;
    public float grabRange = 1f; // Maximum distance to grab objects
    public LayerMask grabbableLayer; // Layer for grabbable objects
    private Rigidbody2D grabbedObject; // Currently grabbed object's Rigidbody2D
    private float objGravity;
    private LayerMask layerState;
    private Movement moveScript;
    private SpringJoint2D joint;

    private void Awake()
    {
        moveScript = GetComponent<Movement>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Grab/Release toggle
        {
            if (grabbedObject == null && moveScript.CheckGrounded())
            {
                TryGrabObject();
            }
            else
            {
                ReleaseObject();
            }
        }
    }

    private void LateUpdate()
    {
        if (grabbedObject != null)
        {
            Transform target = grabbedObject.gameObject.transform;
            // Calculate direction from arm bone to the object
            Vector2 direction = target.position - armBone01.position;
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
            armBone01.rotation = Quaternion.Euler(0, 0, angle);
            armBone02.rotation = Quaternion.Euler(0, 0, angle);

            Debug.DrawLine(armBone01.position, target.position, Color.red);  // First arm to target
            Debug.DrawLine(armBone02.position, target.position, Color.green); // Second arm to target
        }
    }

    void TryGrabObject()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(grabPoint.position, grabRange, grabbableLayer);

        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {   
                grabbedObject = rb;

                objGravity = grabbedObject.gravityScale;
                grabbedObject.gravityScale = 1f;

                layerState = grabbedObject.gameObject.layer;
                grabbedObject.gameObject.layer = LayerMask.NameToLayer("Grabbed Layer");
                //grabbedObject.velocity = Vector2.zero; // Stop movement
                ToggleArmVisibility();
                moveScript.SetMoveSpeedMultiplier(1 / grabbedObject.mass);
                CreateJoint(grabbedObject.ClosestPoint(transform.position));
                return;
            }
        }
    }

    void ReleaseObject()
    {
        if (grabbedObject != null)
        {
            ToggleArmVisibility();
            grabbedObject.gameObject.layer = layerState; // Reset layer
            grabbedObject.gravityScale = objGravity; // Restore gravity
            Destroy(joint);
            grabbedObject = null;
            moveScript.ResetMoveSpeed();
        }
    }

    void ToggleArmVisibility()
    {
        foreach (GameObject arm in animationArms)
        {
            arm.SetActive(!arm.activeSelf);
        }
        foreach (GameObject arm in grabArms)
        {
            arm.SetActive(!arm.activeSelf);
        }
    }

    public bool HoldingObject()
    {
        if (grabbedObject != null)
        {
            return true;
        }
        return false;
    }

    void CreateJoint(Vector2 latchPoint)
    {
        if (grabbedObject == null) return;

        // Add a Distance Joint to the player
        joint = gameObject.AddComponent<SpringJoint2D>();
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