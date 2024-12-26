using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    private Death death;
    // Start is called before the first frame update
    void Awake()
    {
        death = GameObject.FindGameObjectWithTag("Player").GetComponent<Death>();
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag == "Player") 
        {
            Debug.Log("TriggerDeath");
            death.TriggerDeath();
        }

    }
}
