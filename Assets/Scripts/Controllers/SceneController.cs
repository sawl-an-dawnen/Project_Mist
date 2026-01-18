using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public GameManager gameManager;
    public PauseController pauseController;
    public TitleScreenController titleScreenController;
    public SpawnController spawnController;
    public GameObject checkpoint = null;

    //UI
    public UIUtility uiUtility;
    //private Coroutine currentTransition;
    //private float duration = 2f;

    void Awake()
    {
        Debug.Log("Scene Controller Activated: " + this);
        gameManager = GameManager.Instance;
        GameObject temp = GameObject.FindWithTag("GameController");
        pauseController = temp.GetComponent<PauseController>();
        titleScreenController = temp.GetComponent<TitleScreenController>();
        spawnController = temp.GetComponent<SpawnController>();
        uiUtility = temp.GetComponent<UIUtility>();
    }

    public void TriggerInversion()
    {
        IInvertable[] objs = GameObject.FindObjectsOfType<MonoBehaviour>(true).OfType<IInvertable>().ToArray();
        gameManager.Invert();
        foreach (IInvertable obj in objs)
        {
            obj.Invert();
        }
    }
    public void SceneControllerReset() 
    {
        Debug.Log("SCR: Active");
        gameManager = GameManager.Instance;
        gameManager.ResetManager();
        GameObject temp = GameObject.FindWithTag("GameController");
        pauseController = temp.GetComponent<PauseController>();
        //pauseController.ResetController();
        uiUtility = temp.GetComponent<UIUtility>();
        uiUtility.ResetUtility();
        titleScreenController = temp.GetComponent<TitleScreenController>();
        titleScreenController.ResetScene();
        spawnController = temp.GetComponent<SpawnController>();
        spawnController.ResetController();
        spawnController.Spawn();
        Debug.Log("SCR: Complete");
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
    public void ResetGame()
    {
        if (gameManager.Paused())
        {
            pauseController.Resume();
        }
        StartCoroutine(ResetCoroutine());
    }
    public void DelayedResetGame(float t)
    {
        if (gameManager.Paused())
        {
            pauseController.Resume();
        }
        StartCoroutine(DelayedResetCoroutine(t));
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
        if (gameManager.Inverted())
        {
            TriggerInversion();
        }
        uiUtility.FadeToBlack();
        yield return new WaitForSeconds(4f);
        yield return StartCoroutine(LoadSceneAsync(gameManager.GetLevel()));
        SceneControllerReset();
        yield return null;
    }

    public IEnumerator PrepNextScene(string sceneName) 
    {
        Debug.Log("LNS: Remove Control");
        gameManager.SetInControl(false);
        gameManager.SetNewSession(false);
        if (gameManager.Inverted())
        {
            TriggerInversion();
        }
        Debug.Log("LNS: FadeTOBlack");
        uiUtility.FadeToBlack();
        gameManager.SetLevel(sceneName);
        Debug.Log("LNS: set level ->" + gameManager.GetLevel());
        yield return new WaitForSeconds(4f);
        CleanupDontDestroy();
    }
    public IEnumerator DelayedResetCoroutine(float t)
    {
        gameManager.SetInControl(false);
        if (gameManager.Inverted())
        {
            TriggerInversion();
        }
        yield return new WaitForSeconds(t);
        uiUtility.FadeToBlack();
        yield return new WaitForSeconds(4f);
        yield return StartCoroutine(LoadSceneAsync(gameManager.GetLevel()));
        SceneControllerReset();
        yield return null;
    }
    public IEnumerator CloseGameCoroutine()
    {
        gameManager.SetInControl(false);
        if (gameManager.Inverted())
        {
            TriggerInversion();
        }
        uiUtility.FadeToBlack();
        yield return new WaitForSeconds(4f);
        Application.Quit();
        yield return null;
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
    private void CleanupDontDestroy()
    {
        Debug.Log("[SceneController] Cleaning DontDestroyOnLoad scene...");

        // Create a temporary scene to move persistent objects into
        Scene tempScene = SceneManager.CreateScene("TempCleanupScene");

        // Find all root GameObjects, including those in DontDestroyOnLoad
        GameObject[] rootObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in rootObjects)
        {
            if (obj.scene.name == "DontDestroyOnLoad")
            {
                Debug.Log($"[SceneController] Destroying persistent object: {obj.name}");
                SceneManager.MoveGameObjectToScene(obj, tempScene);
                Destroy(obj);
            }
        }
    }
    void OnDestroy()
    {
        Debug.Log("SceneController destroyed!" + this);
    }
}
