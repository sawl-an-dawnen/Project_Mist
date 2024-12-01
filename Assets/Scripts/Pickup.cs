using UnityEngine;

public class Pickup : Interactable
{
    public float weight = 1f;
    
    private bool held = false;
    private Transform rightArmBoneHolding; // The main bone for the arm
    private Transform leftArmBoneHolding; // bone for left arm
    private GameObject[] animationArmSprites; //array arms used for animation
    private GameObject[] holdingArmSprites; //array arms used for holding obj
    private float objGravityScale; //objs original gravity scale
    private LayerMask layerState; //objs orignal layerstate
    private Rigidbody2D grabbedObject; // Currently grabbed object's Rigidbody2D
    private Rigidbody2D player; // players rigidbody 2D
    private Transform holdPoint; // A point near the character where the object will be held
    private Movement moveScript; //plauers movescript

    private void Awake()
    {
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
        if (held)
        {
            // Move the object with the grab point
            grabbedObject.MovePosition(holdPoint.position + (.1f * Vector3.forward * Mathf.Sign(transform.position.x)));

            if ((grabbedObject.transform.position.x - player.transform.position.x) * Mathf.Sign(player.gameObject.transform.localScale.x) <= 0.08f)
            {
                //gameObject.transform.position = new Vector3 (gameObject.transform.position.x - (.01f * Mathf.Sign(transform.localScale.x)), gameObject.transform.position.y, gameObject.transform.position.z);
                player.AddForce(12f * Vector3.right * -Mathf.Sign(player.gameObject.transform.position.x));
            }
        }
    }

    private void LateUpdate()
    {
        if (held)
        {
            Transform target = grabbedObject.gameObject.transform;

            // Calculate direction from arm bone to the object
            Vector2 direction = target.position - rightArmBoneHolding.position;
            float facingMultiplier = gameObject.transform.localScale.x >= 0 ? 1 : -1; // Check if facing right (positive scale) or left (negative scale)
            bool isFacingRight = player.transform.localScale.x >= 0;

            // Convert direction to an angle
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            // Adjust the angle for flipping
            if (!isFacingRight) {angle = 180 - angle;}
            angle *= facingMultiplier;

            // Apply rotation to the arm bone
            rightArmBoneHolding.rotation = Quaternion.Euler(0, 0, angle);
            leftArmBoneHolding.rotation = Quaternion.Euler(0, 0, angle);

            Debug.DrawLine(rightArmBoneHolding.position, target.position, Color.red);  // First arm to target
            Debug.DrawLine(leftArmBoneHolding.position, target.position, Color.green); // Second arm to target
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

        moveScript.SetMoveSpeedMultiplier(1/weight);
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
}
