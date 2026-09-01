using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndScene : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text byText;
    public TMP_Text thankYouText;

    public string firstLevelName = "Level1";

    public float fadeDuration = 3f;
    public float pauseAfterFadeIn = 2f;

    private void Start()
    {
        StartCoroutine(EndSequence());
    }

    private IEnumerator EndSequence()
    {
        // Make everything invisible
        SetAlpha(titleText, 0f);
        SetAlpha(byText, 0f);
        SetAlpha(thankYouText, 0f);

        // Title fades in
        yield return Fade(titleText, 0f, 1f, fadeDuration);

        yield return new WaitForSeconds(pauseAfterFadeIn);

        // By statement fades in
        yield return Fade(byText, 0f, 1f, fadeDuration);

        yield return new WaitForSeconds(pauseAfterFadeIn);

        // Title and by statement fade out together
        yield return FadeTogether(titleText, byText, 1f, 0f, fadeDuration);

        // Thank you fades in
        yield return Fade(thankYouText, 0f, 1f, fadeDuration);

        yield return new WaitForSeconds(pauseAfterFadeIn);

        // Thank you fades out
        yield return Fade(thankYouText, 1f, 0f, fadeDuration);

        // Return to first level
        SceneManager.LoadScene(firstLevelName);
    }

    private IEnumerator Fade(TMP_Text text, float start, float end, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;

            // Very slow at first, then rapidly reaches 1
            t = Mathf.Pow(t, 4f);

            float alpha = Mathf.Lerp(start, end, t);

            SetAlpha(text, alpha);

            yield return null;
        }

        SetAlpha(text, end);
    }

    private IEnumerator FadeTogether(
        TMP_Text text1,
        TMP_Text text2,
        float start,
        float end,
        float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(start, end, time / duration);

            SetAlpha(text1, alpha);
            SetAlpha(text2, alpha);

            yield return null;
        }

        SetAlpha(text1, end);
        SetAlpha(text2, end);
    }

    private void SetAlpha(TMP_Text text, float alpha)
    {
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }
}