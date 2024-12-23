using UnityEngine;


//Archived script - Originally designed to exist on player character. Changed to use abstract class and moved to objects
public class Archived_Pickup : MonoBehaviour
{

    public Transform armBone01; // The main bone for the arm
    public Transform armBone02; // The main bone for the arm
    public Transform grabPoint;
    public Transform holdPoint; // A point near the character where the object will be held
    public GameObject[] animationArms;
    public GameObject[] grabArms;

    public float grabRange = 1f; // Maximum distance to grab objects
    public LayerMask grabbableLayer; // Layer for grabbable objects

    private float objGravityScale;
    private Rigidbody2D grabbedObject; // Currently grabbed object's Rigidbody2D
    private LayerMask layerState;

    private Movement moveScript;

    private void Awake()
    {
        moveScript = GetComponent<Movement>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Grab/Release toggle
        {
            if (grabbedObject == null)
            {
                TryGrabObject();
            }
            else
            {
                ReleaseObject();
            }
        }

        if (grabbedObject != null)
        {
            // Move the object with the grab point
            grabbedObject.MovePosition(holdPoint.position + ( .1f * Vector3.forward * Mathf.Sign(transform.position.x)));

            if ((grabbedObject.transform.position.x - gameObject.transform.position.x) * Mathf.Sign(transform.localScale.x) <= 0.08f) 
            {
                Debug.Log("TOO CLOSE!");
                //gameObject.transform.position = new Vector3 (gameObject.transform.position.x - (.01f * Mathf.Sign(transform.localScale.x)), gameObject.transform.position.y, gameObject.transform.position.z);
                gameObject.GetComponent<Rigidbody2D>().AddForce(12f * Vector3.right * -Mathf.Sign(transform.position.x));
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

            Debug.Log("Rotation Applied");
            Debug.DrawLine(armBone01.position, target.position, Color.red);  // First arm to target
            Debug.DrawLine(armBone02.position, target.position, Color.green); // Second arm to target
        }
    }

    void TryGrabObject()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(grabPoint.position, grabRange, grabbableLayer);

        if (hits.Length > 0)
        {
            grabbedObject = hits[0].attachedRigidbody;

            if (grabbedObject != null)
            {
                ToggleArmVisibility();
                layerState = grabbedObject.gameObject.layer;  
                Debug.Log(layerState);
                grabbedObject.gameObject.layer = LayerMask.NameToLayer("Grabbed Layer");
                objGravityScale = grabbedObject.gravityScale;     
                grabbedObject.gravityScale = 0f; // Disable gravity
                grabbedObject.velocity = Vector2.zero; // Stop movement
                moveScript.SetMoveSpeedMultiplier(1 / grabbedObject.mass);
            }
        }
    }

    void ReleaseObject()
    {
        if (grabbedObject != null)
        {
            ToggleArmVisibility();
            grabbedObject.gameObject.layer = layerState; // Reset layer
            grabbedObject.gravityScale = objGravityScale; // Restore gravity
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

    void OnDrawGizmos()
    {
        // Visualize grab range in the editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(grabPoint.position, grabRange);
    }
}