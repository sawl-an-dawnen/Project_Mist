using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour
{
    public GameObject ragdoll;
    public GameObject sprite;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerDeath() 
    {
        GameObject clone = Object.Instantiate(ragdoll, gameObject.transform.position, gameObject.transform.rotation, gameObject.transform);
        clone.GetComponent<Rigidbody2D>();
        Destroy(sprite);
    }
}
