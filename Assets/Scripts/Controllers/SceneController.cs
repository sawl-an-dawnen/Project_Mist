using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.XR;

public class SceneController : MonoBehaviour
{
    private GameManager gameManager;
    private PauseController pauseController;
    //private TitleScreenController titleScreenController;
    private GameObject checkpoint = null;

    //UI
    private UIUtility uiUtility;
    private Coroutine currentTransition;
    private float duration = 2f;

    void Awake()
    {
        gameManager = GameManager.Instance;
        GameObject temp = GameObject.FindWithTag("GameController");
        pauseController = temp.GetComponent<PauseController>();
        //titleScreenController = temp.GetComponent<TitleScreenController>();
        uiUtility = temp.GetComponent<UIUtility>();
    }
    public void SceneControllerReset() 
    {
        gameManager = GameManager.Instance;
        pauseController = GameObject.FindWithTag("GameController").GetComponent<PauseController>();
    }
    public GameObject GetCheckpoint() {
        if (checkpoint == null) { return null; }
        else { return checkpoint; } 
    }
    public void SetCheckpoint(GameObject checkpoint)
    {
        Debug.Log("Check point set! " + checkpoint.name);
        this.checkpoint = checkpoint;
        gameManager.Save();
    }
    public void CloseGame()
    {
        if (gameManager.Paused())
        {
            pauseController.Resume();
        }
        StartCoroutine(CloseGameCoroutine());
    }
    public IEnumerator ResetCoroutine()
    {
        gameManager.SetInControl(false);
        //StartCoroutine(FadeOutScene());
        uiUtility.FadeToBlack();
        yield return new WaitForSeconds(4f);
        yield return StartCoroutine(LoadSceneAsync(gameManager.GetLevel()));
        gameManager.Reset();
        //titleScreenController.RespawnTrigger();
        SceneControllerReset();
        uiUtility.FadeIn();
        //yield return StartCoroutine(FadeInScene());
        yield return new WaitForSeconds(2f);
        gameManager.SetInControl(true);
        yield return null;
    }
    public IEnumerator CloseGameCoroutine()
    {
        gameManager.SetInControl(false);
        uiUtility.FadeToBlack();
        yield return new WaitForSeconds(4f);
        Application.Quit();
        yield return null;
    }
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Optional: don't allow the scene to activate until we're ready
        asyncLoad.allowSceneActivation = false;

        // Wait until the scene is fully loaded (progress reaches 0.9)
        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                // Scene is ready. Perform your logic here, then allow activation.
                Debug.Log("Scene loaded. Performing post-load logic...");

                // Activate the scene
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        // Optional: Do something immediately after activation
        Debug.Log("Scene activated.");
    }
    IEnumerator Respawn()
    {
        Debug.Log("TSC: Respawn() Triggered...");
        uiUtility.FadeIn();
        yield return new WaitForSeconds(2f);
        //uiUtility.TransitionRawImage(vignette);
        //player.TriggerWakeUp();
    }
}
