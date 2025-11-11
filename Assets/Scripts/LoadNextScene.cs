using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextScene : MonoBehaviour
{
    public string nextLevel;
    public bool activeOnNextScene = true;
    private GameManager gameManager;
    private SceneController sceneController;
    private SpriteRenderer backupBlack;
    // Start is called before the first frame update
    void Awake()
    {
        gameManager = GameManager.Instance;
        GameObject temp = GameObject.FindWithTag("GameController");
        sceneController = temp.GetComponent<SceneController>();
        backupBlack = GameObject.FindWithTag("UI_Backup_Black").GetComponent<SpriteRenderer>();  
    }
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        Debug.Log($"Loading scene async: {sceneName}");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        if (asyncLoad == null)
        {
            Debug.LogError("LoadSceneAsync returned null! Check scene name or build settings.");
            yield break;
        }

        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            Debug.Log($"Loading progress: {asyncLoad.progress}, allowSceneActivation: {asyncLoad.allowSceneActivation}");
            if (asyncLoad.progress >= 0.9f)
            {
                Debug.Log("Scene loaded to 90%, activating...");
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }

        Debug.Log("Scene activated successfully.");
    }

    public IEnumerator TransitionToNextScene() 
    {
        gameManager.SetActiveOnNextRespawn(activeOnNextScene);
        yield return StartCoroutine(sceneController.PrepNextScene(nextLevel));
        backupBlack.enabled = true;
        yield return LoadSceneAsync(nextLevel);
    }

    private void OnTriggerEnter2D(Collider2D other) => StartCoroutine(TransitionToNextScene());

}
