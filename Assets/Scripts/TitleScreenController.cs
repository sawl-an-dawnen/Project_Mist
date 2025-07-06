using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleScreenController : MonoBehaviour
{
    private GameObject titleScreen;
    private RawImage vignette;
    private TextFadeInPulse titleText;
    private TextFadeInPulse instructionText;
    private UIController uiController;
    private WakeUp player;
    private bool inControl = false;
    private bool readyToStart = false;
    private bool titleSequenceComplete = false;


    void Awake() {
        titleScreen = GameObject.FindWithTag("UI_Title");
        vignette = GameObject.FindWithTag("UI_Vignette").GetComponent<RawImage>();
        titleText = GameObject.FindWithTag("UI_TitleText").GetComponent<TextFadeInPulse>();
        instructionText = GameObject.FindWithTag("UI_TitleInstructions").GetComponent<TextFadeInPulse>();
        uiController = GameObject.FindWithTag("GameController").GetComponent<UIController>();
        player = GameObject.FindWithTag("Player_Wakeup").GetComponent<WakeUp>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if (!titleSequenceComplete)
        {
            StartCoroutine(TitleSequence());
        }
        else {
            StartCoroutine(Respawn());
        }
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

    IEnumerator TitleSequence()
    {
        Debug.Log("Enter Title Sequence...");
        uiController.FadeIn();
        // Wait for delay
        yield return new WaitForSeconds(4f);
        titleText.Trigger();
        yield return new WaitForSeconds(4f);
        inControl = true;
        instructionText.Trigger();
        yield return null;
    }

    IEnumerator StartingGame() {
        Debug.Log("Starting game...");
        titleText.FadeAway();
        instructionText.FadeAway();
        uiController.FadeRawImage(vignette);
        yield return new WaitForSeconds(2f);
        player.TriggerWakeUp();
        titleSequenceComplete = true;
        yield return null;
    }
    IEnumerator Respawn() {
        uiController.FadeIn();
        // Wait for delay
        yield return new WaitForSeconds(2f);
        uiController.FadeRawImage(vignette);
        inControl = true;
        player.TriggerWakeUp();
    }
}
