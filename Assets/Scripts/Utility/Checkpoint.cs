using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private SceneController controller;

    public void Awake() {
        controller = GameObject.FindWithTag("GameController").GetComponent<SceneController>();
    }
    // Called when another collider enters this trigger (2D only)
    private void OnTriggerEnter2D(Collider2D other)
    {
        controller.SetCheckpoint(gameObject);
    }
}
