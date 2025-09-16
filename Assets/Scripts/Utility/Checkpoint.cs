using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool lockInvertAbility = false;
    private SceneController controller;
    private GameManager gameManager = GameManager.Instance;

    public void Awake() {
        controller = GameObject.FindWithTag("GameController").GetComponent<SceneController>();
    }
    // Called when another collider enters this trigger (2D only)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) {
            controller.SetCheckpoint(gameObject);
            if (lockInvertAbility) {
                gameManager.SetInvertAbilityLock(true);
            }
        }
    }
}
