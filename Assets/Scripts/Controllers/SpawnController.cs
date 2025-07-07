using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public Transform defaultSpawn;
    private Transform spawn = null;
    private Transform player;
    private SceneController sceneController;

    // Start is called before the first frame update
    void Awake()
    {
        sceneController = GameObject.FindWithTag("GameController").GetComponent<SceneController>();
        player = GameObject.FindWithTag("Player_wrapper").transform;
    }

    void Start() {
        GameObject tempSpawn = sceneController.GetCheckpoint();

        if (tempSpawn != null) {
            spawn = tempSpawn.transform;
        }
        if (spawn == null) {
            spawn = defaultSpawn;
        }

        player.position = spawn.position;
    }
}
