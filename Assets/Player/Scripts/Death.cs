using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class Death : MonoBehaviour
{
    public GameObject ragdollPrefab;
    public Transform mainParent;

    private Ragdoll ragdoll;
    private Rigidbody2D target;
    private Rigidbody2D source;

    private CinemachineVirtualCamera m_Camera;
    private SceneController sceneController;
    // Start is called before the first frame update
    void Awake()
    {
        source = GetComponent<Rigidbody2D>();
        m_Camera = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        sceneController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneController>();
    }

    public void TriggerDeath(float targetGravity, float resetDelay = 2f)
    {
        GameObject clone = Object.Instantiate(ragdollPrefab, gameObject.transform.position, gameObject.transform.rotation, mainParent);
        target = clone.GetComponent<Rigidbody2D>();
        m_Camera.Follow = clone.transform;
        ragdoll = clone.GetComponent<Ragdoll>();

        ragdoll.leftArm.GetComponent<Rigidbody2D>().gravityScale = targetGravity;
        ragdoll.rightArm.GetComponent<Rigidbody2D>().gravityScale = targetGravity;
        ragdoll.leftLeg.GetComponent<Rigidbody2D>().gravityScale = targetGravity;
        ragdoll.rightLeg.GetComponent<Rigidbody2D>().gravityScale = targetGravity;
        ragdoll.body.GetComponent<Rigidbody2D>().gravityScale = targetGravity;
        ragdoll.head.GetComponent<Rigidbody2D>().gravityScale = targetGravity;

        // Copy velocity-related properties
        target.velocity = source.velocity;
        target.angularVelocity = source.angularVelocity;
        target.AddForce(Vector2.up*2f,ForceMode2D.Impulse);

        sceneController.DelayedResetGame(resetDelay);
        Destroy(gameObject);
    }

    public void TriggerDeath(float resetDelay = 2f) 
    {
        GameObject clone = Object.Instantiate(ragdollPrefab, gameObject.transform.position, gameObject.transform.rotation, mainParent);
        target = clone.GetComponent<Rigidbody2D>();
        m_Camera.Follow = clone.transform;

        target.velocity = source.velocity;
        target.angularVelocity = source.angularVelocity;

        sceneController.DelayedResetGame(resetDelay);
        Destroy(gameObject);
    }
}
