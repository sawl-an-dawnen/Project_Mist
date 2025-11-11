using System.Collections;
using UnityEngine;


public class TitleScreenController : MonoBehaviour
{
    public bool guarunteeInversionControl = true;
    public bool skipTitle = false;
    private TextFadeInPulse titleText;
    private TextFadeInPulse instructionText;
    private UIUtility uiController;
    private GameManager gameManager;
    private WakeUp player;
    private bool inControl = false;
    private bool readyToStart = false;

    void Awake() {
        //titleScreen = GameObject.FindWithTag("UI_Title");
        titleText = GameObject.FindWithTag("UI_TitleText").GetComponent<TextFadeInPulse>();
        instructionText = GameObject.FindWithTag("UI_TitleInstructions").GetComponent<TextFadeInPulse>();
        uiController = GameObject.FindWithTag("GameController").GetComponent<UIUtility>();
        gameManager = GameManager.Instance;
        player = GameObject.FindWithTag("Player_Wakeup").GetComponent<WakeUp>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if (guarunteeInversionControl) {
            gameManager.SetCanInvert(true);
            gameManager.SetInvertAbilityLock(true);
        }
        if (skipTitle || !gameManager.IsNewSession()) {
            StartCoroutine(RespawnSequence());
            return;
        }
        StartCoroutine(TitleSequence());
    }
    // Update is called once per frame
    void Update()
    {
        if (inControl && !readyToStart) {
            readyToStart = true;
        }
        if (readyToStart) {
            if (Input.anyKeyDown)
            {
                StartCoroutine(StartingGame());
                readyToStart = false;
                inControl = false;
            }
        }
    }

    public void ResetScene() {
        Debug.Log("TitleScreenReset");
        titleText = GameObject.FindWithTag("UI_TitleText").GetComponent<TextFadeInPulse>();
        instructionText = GameObject.FindWithTag("UI_TitleInstructions").GetComponent<TextFadeInPulse>();
        uiController = GameObject.FindWithTag("GameController").GetComponent<UIUtility>();
        gameManager = GameManager.Instance;
        player = GameObject.FindWithTag("Player_Wakeup").GetComponent<WakeUp>();
        StartCoroutine(RespawnSequence());
    }

    IEnumerator TitleSequence()
    {
        uiController.FadeIn();
        // Wait for delay
        yield return new WaitForSeconds(4f);
        titleText.Trigger();
        yield return new WaitForSeconds(4f);
        inControl = true;
        instructionText.Trigger();
        yield return null;
    }
    IEnumerator RespawnSequence()
    {
        if (gameManager.ActiveOnNextRespawnCheck())
        {
            player.TriggerWakeUp();
            uiController.FadeIn();
            yield return new WaitForSeconds(1f);
            uiController.TransitionRawImage(uiController.GetVignette());
            yield return null;
        }
        else 
        {
            uiController.FadeIn();
            yield return new WaitForSeconds(2f);
            uiController.TransitionRawImage(uiController.GetVignette());
            yield return new WaitForSeconds(2f);
            player.TriggerWakeUp();
            yield return null;
        }
    }
    IEnumerator StartingGame() 
    {
        titleText.FadeAway();
        instructionText.FadeAway();
        uiController.TransitionRawImage(uiController.GetVignette());
        yield return new WaitForSeconds(2f);
        player.TriggerWakeUp();
        yield return null;
    }
}
