using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadNextScene : MonoBehaviour
{
    public string nextLevel;
    private SceneController sceneController;
    // Start is called before the first frame update
    void Awake()
    {
        GameObject temp = GameObject.FindWithTag("GameController");
        sceneController = temp.GetComponent<SceneController>();
    }

    private void OnTriggerEnter2D(Collider2D other) => StartCoroutine(sceneController.LoadNextScene(nextLevel));

}
