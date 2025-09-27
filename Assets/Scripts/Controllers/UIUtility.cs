using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIUtility : MonoBehaviour
{
    public float transitionDuration = 3f;
    private RawImage blackScreen;
    private RawImage vignette;

    void Awake() {
        blackScreen = GameObject.FindWithTag("UI_Black").GetComponent<RawImage>();
        vignette = GameObject.FindWithTag("UI_Vignette").GetComponent<RawImage>();
    }

    public void ResetUtility()
    {
        blackScreen = GameObject.FindWithTag("UI_Black").GetComponent<RawImage>();
        vignette = GameObject.FindWithTag("UI_Vignette").GetComponent<RawImage>();
    }

    public RawImage GetBlackScreen() { return blackScreen;}
    public RawImage GetVignette() { return vignette;}

    public void TransitionTMP(TextMeshProUGUI tmp, float alpha = 0f, float duration= 1f, float wait = 1f) {
        StartCoroutine(TransitionTMPCoroutine(tmp, alpha, duration, wait));
    }

    public void TransitionRawImage(RawImage tmp, float alpha = 0f, float duration = 1f, float wait = 1f)
    {
        StartCoroutine(TransitionRawImageCoroutine(tmp, alpha, duration, wait));
    }

    public void FadeToBlack(float duration = -1)
    {
        blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 0f);
        if (duration <= -1)
        {
            StartCoroutine(TransitionRawImageCoroutine(blackScreen, 1f, transitionDuration, 0f));
        }
        else 
        {
            StartCoroutine(TransitionRawImageCoroutine(blackScreen, 1f, duration, 0f));
        }
    }

    public void FadeIn(float duration = -1, float wait = 1f) 
    {
        blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 1f);
        if (duration <= -1)
        {
            StartCoroutine(TransitionRawImageCoroutine(blackScreen, 0f, transitionDuration, wait));
        }
        else
        {
            StartCoroutine(TransitionRawImageCoroutine(blackScreen, 0f, duration, wait));
        }
    }

    #region Coroutines

    IEnumerator TransitionTMPCoroutine(TextMeshProUGUI tmp, float alpha, float duration, float wait)
    {
        if (tmp == null) yield break;

        yield return new WaitForSeconds(wait);
        float elapsed = 0f;
        Color originalColor = tmp.color;
        float startAlpha = originalColor.a;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float alphaTarget = Mathf.Lerp(startAlpha, alpha, t);
            tmp.color = new Color(originalColor.r, originalColor.g, originalColor.b, alphaTarget);
            yield return null;
        }

        tmp.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha); // ensure final value
    }

    IEnumerator TransitionRawImageCoroutine(RawImage tmp, float alpha, float duration, float wait)
    {
        if (tmp == null) yield break;

        yield return new WaitForSeconds(wait);

        float elapsed = 0f;
        Color originalColor = tmp.color;
        float startAlpha = originalColor.a;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float alphaTarget = Mathf.Lerp(startAlpha, alpha, t);
            tmp.color = new Color(originalColor.r, originalColor.g, originalColor.b, alphaTarget);
            yield return null;
        }

        // Ensure the final alpha is exactly set
        tmp.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
    }

    #endregion
}
