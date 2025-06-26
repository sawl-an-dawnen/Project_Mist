using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    private GameManager gameManager;
    private GameObject checkpoint;


    //UI
    private GameObject titleScreen;
    private GameObject pauseMenu;
    private GameObject settingsMenu;
    private RawImage blackScreen;

    private Coroutine currentTransition;
    private float duration = 1f;

    void Awake()
    {
        titleScreen = GameObject.FindWithTag("UI_Title");
        pauseMenu = GameObject.FindWithTag("UI_Pause");
        settingsMenu = GameObject.FindWithTag("UI_Settings");
        blackScreen = GameObject.FindWithTag("UI_Black").GetComponent<RawImage>();
        gameManager = GameManager.Instance;
    }

    public GameObject GetCheckpoint() { return checkpoint; }
    public void SetCheckpoint(GameObject checkpoint)
    {
        this.checkpoint = checkpoint;
        gameManager.Save();
    }

    private IEnumerator LoadCheckPoint(Color targetColor)
    {
        yield return StartCoroutine(FadeOutScene());
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FadeInScene());
    }

    //fade in
    public IEnumerator FadeInScene()
    {
        if (currentTransition != null)
        {
            StopCoroutine(currentTransition);
        }

        currentTransition = StartCoroutine(TransitionColorCoroutine(new Color(0f, 0f, 0f, 0f)));
        yield return null;
    }
    //fade out
    public IEnumerator FadeOutScene()
    {
        if (currentTransition != null)
        {
            StopCoroutine(currentTransition);
        }

        currentTransition = StartCoroutine(TransitionColorCoroutine(new Color(0f, 0f, 0f, 1f)));
        yield return null;
    }

    private IEnumerator TransitionColorCoroutine(Color targetColor)
    {
        Color startColor = blackScreen.color;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            blackScreen.color = Color.Lerp(startColor, targetColor, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        blackScreen.color = targetColor; // Ensure it finishes exactly at target
        currentTransition = null;
    }
}
