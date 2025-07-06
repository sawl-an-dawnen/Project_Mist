using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIController : MonoBehaviour
{
    public float fadeDuration = 2f;
    private RawImage blackScreen;


    void Awake() {
        blackScreen = GameObject.FindWithTag("UI_Black").GetComponent<RawImage>();
    }
    /// <summary>
    /// Fade color to designated alpha designed for TMPro
    /// </summary>
    public void FadeTMP(TextMeshProUGUI tmp, float alpha = 0f, float duration=1f, float wait = 1f) {
        StartCoroutine(FadeCoroutineTMP(tmp, alpha, duration, wait));
    }
    /// <summary>
    /// Fade color to designated alpha designed for RawImages
    /// </summary>
    public void FadeRawImage(RawImage tmp, float alpha = 0f, float duration = 1f, float wait = 1f)
    {
        StartCoroutine(FadeCoroutineRawImage(tmp, alpha, duration, wait));
    }
    /// <summary>
    /// Fade out designed for the main black screen of the game
    /// </summary>
    public void FadeToBlack()
    {
        Debug.Log("UI: FadeOut()");
        blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 0f);
        StartCoroutine(FadeCoroutineRawImage(blackScreen, 1f, fadeDuration, 1f));
    }
    /// <summary>
    /// Fade in designed for the main black screen of the game
    /// </summary>
    public void FadeIn() 
    {
        Debug.Log("UI: FadeIn()");
        blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 1f);
        StartCoroutine(FadeCoroutineRawImage(blackScreen, 0f, fadeDuration, 1f));
    }

    #region Coroutines

    IEnumerator FadeCoroutineTMP(TextMeshProUGUI tmp, float alpha, float duration, float wait)
    {
        if (tmp == null) yield break;
        bool running = true;
        duration *= 100f;
        yield return new WaitForSeconds(wait);
        float elapsed = 0f;
        Color originalColor = tmp.color;

        while (elapsed < duration && running)
        {
            elapsed += Time.deltaTime;
            float alphaTarget = Mathf.Lerp(tmp.color.a, alpha, elapsed / duration);
            tmp.color = new Color(originalColor.r, originalColor.g, originalColor.b, alphaTarget);
            Debug.Log(Mathf.Abs(tmp.color.a - alpha));
            if (Mathf.Abs(tmp.color.a - alpha) <= 0.005) { running = false; }
            yield return null;
        }

        // Ensure it's fully visible at the end
        tmp.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
        yield return null;
    }

    IEnumerator FadeCoroutineRawImage(RawImage tmp, float alpha, float duration, float wait)
    {
        if (tmp == null) yield break;
        bool running = true;
        duration *= 1000f;
        yield return new WaitForSeconds(wait);
        float elapsed = 0f;
        Color originalColor = tmp.color;

        while (elapsed < duration && running)
        {
            elapsed += Time.deltaTime;
            float alphaTarget = Mathf.Lerp(tmp.color.a, alpha, elapsed / duration);
            tmp.color = new Color(originalColor.r, originalColor.g, originalColor.b, alphaTarget);
            Debug.Log(Mathf.Abs(tmp.color.a - alpha));
            if (Mathf.Abs(tmp.color.a - alpha) <= 0.005) { running = false; }
            yield return null;
        }

        // Ensure it's fully transparent at the end
        tmp.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
        yield return null;
    }

    #endregion
}
