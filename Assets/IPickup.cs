using UnityEngine;

public class IPickup : Interactable
{
    private bool held = false;

    public Transform armBone01; // The main bone for the arm
    public Transform armBone02; // The main bone for the arm
    public GameObject[] animationArms;
    public GameObject[] grabArms;


    private float objGravityScale;
    private Rigidbody2D grabbedObject; // Currently grabbed object's Rigidbody2D
    private LayerMask layerState;

    private Rigidbody2D player;
    private Transform holdPoint; // A point near the character where the object will be held
    private Movement moveScript;



    private void Awake()
    {
        
        grabbedObject = GetComponent<Rigidbody2D>();
        objGravityScale = grabbedObject.gravityScale;
        layerState = grabbedObject.gameObject.layer;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        moveScript = player.gameObject.GetComponent<Movement>();
        holdPoint = GameObject.FindGameObjectWithTag("HoldPoint").transform;
    }

    void Update()
    {
        /*
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
        */

        if (held)
        {
            // Move the object with the grab point
            grabbedObject.MovePosition(holdPoint.position + (.1f * Vector3.forward * Mathf.Sign(transform.position.x)));

            if ((grabbedObject.transform.position.x - player.transform.position.x) * Mathf.Sign(transform.localScale.x) <= 0.08f)
            {
                Debug.Log("TOO CLOSE!");
                //gameObject.transform.position = new Vector3 (gameObject.transform.position.x - (.01f * Mathf.Sign(transform.localScale.x)), gameObject.transform.position.y, gameObject.transform.position.z);
                player.AddForce(12f * Vector3.right * -Mathf.Sign(player.gameObject.transform.position.x));
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

    public bool HoldingObject()
    {
        return held;
    }

    public override void Interact() 
    {
        held = true;

        ToggleArmVisibility();

        grabbedObject.gameObject.layer = LayerMask.NameToLayer("Grabbed Layer");
        grabbedObject.gravityScale = 0f; // Disable gravity
        grabbedObject.velocity = Vector2.zero; // Stop movement

        moveScript.SetMoveSpeedMultiplier(1 / grabbedObject.mass);
    }

    public override void Release() 
    {
        held = false;

        ToggleArmVisibility();

        grabbedObject.gameObject.layer = layerState; // Reset layer
        grabbedObject.gravityScale = objGravityScale; // Restore gravity

        moveScript.ResetMoveSpeed();
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
}
