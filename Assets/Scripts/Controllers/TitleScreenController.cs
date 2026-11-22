using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenController : MonoBehaviour
{
    public bool guarunteeInversionControl = true;
    public bool skipTitle = false;

    [SerializeField] private TextFadeInPulse titleText;
    [SerializeField] private TextFadeInPulse instructionText;
    [SerializeField] private UIUtility uiController;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private WakeUp player;

    private bool inControl = false;
    private bool readyToStart = false;

    private Coroutine activeSequence;

    void Awake()
    {
        RefreshReferences();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        InitializeTitleScreen();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("[TitleScreenController] Scene loaded: " + scene.name);

        RefreshReferences();
    }

    private void RefreshReferences()
    {
        Debug.Log("[TitleScreenController] Refreshing references...");

        GameObject titleObject = GameObject.FindWithTag("UI_TitleText");
        GameObject instructionObject = GameObject.FindWithTag("UI_TitleInstructions");
        GameObject controllerObject = GameObject.FindWithTag("GameController");
        GameObject playerObject = GameObject.FindWithTag("Player_Wakeup");

        // Only update references when a valid object is found
        if (titleObject != null)
        {
            TextFadeInPulse component =
                titleObject.GetComponent<TextFadeInPulse>();

            if (component != null)
                titleText = component;
        }

        if (instructionObject != null)
        {
            TextFadeInPulse component =
                instructionObject.GetComponent<TextFadeInPulse>();

            if (component != null)
                instructionText = component;
        }

        if (controllerObject != null)
        {
            UIUtility component =
                controllerObject.GetComponent<UIUtility>();

            if (component != null)
                uiController = component;
        }

        if (playerObject != null)
        {
            WakeUp component =
                playerObject.GetComponent<WakeUp>();

            if (component != null)
                player = component;
        }

        if (GameManager.Instance != null)
            gameManager = GameManager.Instance;

        Debug.Log(
            "[TitleScreenController] References: " +
            $"Title={titleText != null}, " +
            $"Instructions={instructionText != null}, " +
            $"UI={uiController != null}, " +
            $"Player={player != null}, " +
            $"GameManager={gameManager != null}"
        );
    }

    private void InitializeTitleScreen()
    {
        if (gameManager == null)
        {
            Debug.LogError("[TitleScreenController] Cannot initialize without GameManager.");
            return;
        }

        if (guarunteeInversionControl)
        {
            gameManager.SetCanInvert(true);
            gameManager.SetInvertAbilityLock(true);
        }

        if (skipTitle || !gameManager.IsNewSession())
        {
            RunSequence(RespawnSequence());
            return;
        }

        RunSequence(TitleSequence());
    }

    void Update()
    {
        if (inControl && !readyToStart)
        {
            readyToStart = true;
        }

        if (readyToStart && Input.anyKeyDown)
        {
            readyToStart = false;
            inControl = false;

            RunSequence(StartingGame());
        }
    }

    private void RunSequence(IEnumerator sequence)
    {
        if (activeSequence != null)
        {
            StopCoroutine(activeSequence);
        }

        activeSequence = StartCoroutine(sequence);
    }

    public void ResetScene()
    {
        Debug.Log("[TitleScreenController] TitleScreenReset");

        inControl = false;
        readyToStart = false;

        RefreshReferences();

        RunSequence(RespawnSequence());
    }

    private IEnumerator TitleSequence()
    {
        Debug.Log("[TitleScreenController] TitleSequence.");
        if (uiController == null ||
            titleText == null ||
            instructionText == null)
        {
            Debug.LogError("[TitleScreenController] Missing title sequence references.");
            yield break;
        }

        uiController.FadeIn();

        yield return new WaitForSeconds(4f);

        titleText.Trigger();

        yield return new WaitForSeconds(4f);

        inControl = true;

        instructionText.Trigger();
    }

    private IEnumerator RespawnSequence()
    {
        Debug.Log("[TitleScreenController] RespawnSqeuence");
        if (gameManager == null ||
            player == null ||
            uiController == null)
        {
            Debug.LogError("[TitleScreenController] Missing respawn references.");
            yield break;
        }

        if (gameManager.ActiveOnNextRespawnCheck())
        {
            player.TriggerWakeUp();

            uiController.FadeIn();

            yield return new WaitForSeconds(1f);

            uiController.TransitionRawImage(
                uiController.GetVignette()
            );
        }
        else
        {
            uiController.FadeIn();

            yield return new WaitForSeconds(2f);

            uiController.TransitionRawImage(
                uiController.GetVignette()
            );

            yield return new WaitForSeconds(2f);

            player.TriggerWakeUp();
        }
    }

    private IEnumerator StartingGame()
    {
        if (titleText == null ||
            instructionText == null ||
            uiController == null ||
            player == null)
        {
            Debug.LogError("[TitleScreenController] Missing starting game references.");
            yield break;
        }

        titleText.FadeAway();
        instructionText.FadeAway();

        uiController.TransitionRawImage(
            uiController.GetVignette()
        );

        yield return new WaitForSeconds(2f);

        player.TriggerWakeUp();
    }
}