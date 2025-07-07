using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    public GameObject[] activate;
    public GameObject[] deactivate;

    public void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag == "Player") 
        {
            foreach (GameObject obj in activate) 
            {
                obj.SetActive(true);
            }
            foreach (GameObject obj in deactivate)
            {
                obj.SetActive(false);
            }
        }
    }
}
