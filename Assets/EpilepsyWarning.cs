using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EpilepsyWarning : MonoBehaviour
{
    public TMP_Text warningText;

    public float fadeDuration = 2f;
    public float pauseAfterFadeIn = 3f;

    private void Start()
    {
        StartCoroutine(WarningSequence());
    }

    private IEnumerator WarningSequence()
    {
        // Start invisible
        SetAlpha(warningText, 0f);

        // Fade warning in
        yield return Fade(warningText, 0f, 1f, fadeDuration);

        // Keep warning on screen
        yield return new WaitForSeconds(pauseAfterFadeIn);

        // Fade warning out
        yield return Fade(warningText, 1f, 0f, fadeDuration);

        // Load the save data
        GameManager.Instance.Load();

        // Get the last level the player was on
        string levelToLoad = GameManager.Instance.GetLevel();

        // Load that level
        SceneManager.LoadScene(levelToLoad);
    }

    private IEnumerator Fade(TMP_Text text, float start, float end, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;

            // Slow at first, then speeds up
            t = Mathf.Pow(t, 4f);

            float alpha = Mathf.Lerp(start, end, t);

            SetAlpha(text, alpha);

            yield return null;
        }

        SetAlpha(text, end);
    }

    private void SetAlpha(TMP_Text text, float alpha)
    {
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }
}
