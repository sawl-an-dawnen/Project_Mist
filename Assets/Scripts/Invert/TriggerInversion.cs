using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerInversion : MonoBehaviour
{
    public bool destroyAfterTrigger = false;
    private SceneController controller;
    void Awake()
    {
        controller = GameObject.FindWithTag("GameController").GetComponent<SceneController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            controller.TriggerInversion();
            if (destroyAfterTrigger)
            {
                Destroy(gameObject);
            }
        }
    }
}
