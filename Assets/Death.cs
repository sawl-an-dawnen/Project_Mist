using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class Death : MonoBehaviour
{
    public GameObject ragdollPrefab;

    private Ragdoll ragdoll;
    private Rigidbody2D target;
    private Rigidbody2D source;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<Rigidbody2D>();
    }

    public void TriggerDeath(float targetGravity)
    {
        GameObject clone = Object.Instantiate(ragdollPrefab, gameObject.transform.position, gameObject.transform.rotation);
        target = clone.GetComponent<Rigidbody2D>();


        ragdoll = clone.GetComponent<Ragdoll>();

        ragdoll.leftArm.GetComponent<Rigidbody2D>().gravityScale = targetGravity;
        ragdoll.rightArm.GetComponent<Rigidbody2D>().gravityScale = targetGravity;
        ragdoll.leftLeg.GetComponent<Rigidbody2D>().gravityScale = targetGravity;
        ragdoll.rightLeg.GetComponent<Rigidbody2D>().gravityScale = targetGravity;
        ragdoll.body.GetComponent<Rigidbody2D>().gravityScale = targetGravity;
        ragdoll.head.GetComponent<Rigidbody2D>().gravityScale = targetGravity;

        // Copy velocity-related properties
        //apply slight force up for water!!!
        target.angularVelocity = source.angularVelocity;

        // Copy constraints
        //target.constraints = source.constraints;

        Destroy(gameObject);
    }

    public void TriggerDeath() 
    {
        GameObject clone = Object.Instantiate(ragdollPrefab, gameObject.transform.position, gameObject.transform.rotation);
        target = clone.GetComponent<Rigidbody2D>();

        // Copy velocity-related properties
        target.velocity = source.velocity;
        target.angularVelocity = source.angularVelocity;

        // Copy constraints
        //target.constraints = source.constraints;

        Destroy(gameObject);
    }
}
