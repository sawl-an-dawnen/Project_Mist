using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    public GameObject trap;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TriggerKey"))
        {
            Debug.Log("Trap Triggered: " + trap);
            trap.GetComponent<ITrap>().ActivateTrap();
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("Touching: " + other.name);
    }
}
