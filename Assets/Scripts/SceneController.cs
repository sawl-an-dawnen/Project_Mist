using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private GameManager gameManager;
    private PauseController pauseController;
    private GameObject checkpoint = null;

    //UI
    private RawImage blackScreen;

    private Coroutine currentTransition;
    private float duration = 2f;

    void Awake()
    {
        gameManager = GameManager.Instance;
        pauseController = GameObject.FindWithTag("GameController").GetComponent<PauseController>();
        blackScreen = GameObject.FindWithTag("UI_Black").GetComponent<RawImage>();
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
    private IEnumerator TransitionColorCoroutine(Color targetColor)
    {
        Debug.Log("Transition called: " + targetColor);
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
        yield return null;
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
    public void Reset()
    {
        if (gameManager.Paused()) 
        {
            pauseController.Resume();
        }
        StartCoroutine(ResetCoroutine());
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
        StartCoroutine(FadeOutScene());
        yield return new WaitForSeconds(4f);
        gameManager.Reset();
        SceneManager.LoadScene(gameManager.GetLevel());
        StartCoroutine(FadeInScene());
        yield return null;
    }
    public IEnumerator CloseGameCoroutine()
    {
        gameManager.SetInControl(false);
        StartCoroutine(FadeOutScene());
        yield return new WaitForSeconds(4f);
        Application.Quit();
        yield return null;
    }
}
