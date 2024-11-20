using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Pickup : MonoBehaviour
{

    public Transform armBone01; // The main bone for the arm
    public Transform armBone02; // The main bone for the arm
    public Transform grabPoint; // A point near the character where the object will be held
    public float grabRange = 1f; // Maximum distance to grab objects
    public LayerMask grabbableLayer; // Layer for grabbable objects

    private Rigidbody2D grabbedObject; // Currently grabbed object's Rigidbody2D
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
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
            grabbedObject.MovePosition(grabPoint.position);
        }
    }

    private void LateUpdate()
    {
        if (grabbedObject != null)
        {
            Transform target = grabbedObject.gameObject.transform;
            // Calculate direction from arm bone to the object
            Vector2 direction = target.position - armBone01.position;

            // Convert direction to an angle
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            //angle *= gameObject.transform.localScale.x / Mathf.Abs(gameObject.transform.localScale.x);
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
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, grabRange, grabbableLayer);

        if (hits.Length > 0)
        {
            grabbedObject = hits[0].attachedRigidbody;

            if (grabbedObject != null)
            {
                animator.SetLayerWeight(animator.GetLayerIndex("Arms"), 0);
                grabbedObject.gameObject.layer = LayerMask.NameToLayer("Grabbed Layer");
                grabbedObject.gravityScale = 0f; // Disable gravity
                grabbedObject.velocity = Vector2.zero; // Stop movement
            }
        }
    }

    void ReleaseObject()
    {
        if (grabbedObject != null)
        {
            animator.SetLayerWeight(animator.GetLayerIndex("Arms"), 1);
            grabbedObject.gameObject.layer = LayerMask.NameToLayer("Seizable"); // Reset layer
            grabbedObject.gravityScale = 1f; // Restore gravity
            grabbedObject = null;
        }
    }

    void OnDrawGizmos()
    {
        // Visualize grab range in the editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, grabRange);
    }
}