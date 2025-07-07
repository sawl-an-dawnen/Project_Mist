using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleScreenController : MonoBehaviour
{
    private GameObject titleScreen;
    private TextFadeInPulse titleText;
    private TextFadeInPulse instructionText;
    private UIUtility uiController;
    private WakeUp player;
    private bool inControl = false;
    private bool readyToStart = false;

    void Awake() {
        titleScreen = GameObject.FindWithTag("UI_Title");
        titleText = GameObject.FindWithTag("UI_TitleText").GetComponent<TextFadeInPulse>();
        instructionText = GameObject.FindWithTag("UI_TitleInstructions").GetComponent<TextFadeInPulse>();
        uiController = GameObject.FindWithTag("GameController").GetComponent<UIUtility>();
        player = GameObject.FindWithTag("Player_Wakeup").GetComponent<WakeUp>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
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
    IEnumerator TitleSequence()
    {
        Debug.Log("TSC: Enter Title Sequence...");
        uiController.FadeIn();
        // Wait for delay
        yield return new WaitForSeconds(4f);
        titleText.Trigger();
        yield return new WaitForSeconds(4f);
        inControl = true;
        instructionText.Trigger();
        yield return null;
    }
    IEnumerator StartingGame() 
    {
        Debug.Log("TSC: Starting game...");
        titleText.FadeAway();
        instructionText.FadeAway();
        uiController.TransitionRawImage(uiController.GetVignette());
        yield return new WaitForSeconds(2f);
        player.TriggerWakeUp();
        Destroy(titleScreen);
        Destroy(this);
        yield return null;
    }
}
