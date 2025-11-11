using System.Collections;
using TMPro;
using UnityEngine;

public class TextFadeInPulse : MonoBehaviour
{

    public float delayBeforeFade = 1f;
    public float fadeDuration = 1f;
    public float pulseSpeed = 1.5f;
    public float lowerLimit = 0f;
    public bool startOnStart = false;
    public bool deleteOnFade = true;

    private TextMeshProUGUI text;
    private UIUtility uiUtility;
    private Color originalColor;
    private bool pulsing = true;
    private float amplitude;
    private float offset;

    void Awake() {
        text = gameObject.GetComponent<TextMeshProUGUI>();
        originalColor = text.color;
        text.color = new Color(0f, 0f, 0f, 0f);
        uiUtility = GameObject.FindWithTag("GameController").GetComponent<UIUtility>();
    }

    void Start()
    {
        if (text == null)
        {
            Debug.LogError("TextMeshProUGUI reference not set.");
            return;
        }

        amplitude = (1f - lowerLimit) / 2f;
        offset = lowerLimit + amplitude;
        if (startOnStart) {
            StartCoroutine(FadeInAndPulse());
        }
    }

    public void Trigger() {
        // Start the coroutine
        StartCoroutine(FadeInAndPulse());
    }

    public void FadeAway() {
        pulsing = false;
        uiUtility.TransitionTMP(text, 0f, fadeDuration);
    }

    IEnumerator FadeInAndPulse()
    {
        if (pulsing)
        {
            // Wait for delay
            yield return new WaitForSeconds(delayBeforeFade);

            // Fade in
            float elapsed = 0f;
            while (elapsed < fadeDuration && pulsing)
            {
                float alpha = Mathf.Lerp(0f, originalColor.a, elapsed / fadeDuration);
                text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Ensure final alpha
            text.color = originalColor;
            float time = 0f;

            // Start pulsing
            while (pulsing)
            {
                float pulse = offset + Mathf.Sin(time * pulseSpeed + Mathf.PI / 2f) * amplitude;
                //Debug.Log(this + ": " + pulse);
                text.color = new Color(originalColor.r, originalColor.g, originalColor.b, pulse);
                time += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            yield return null;
        }
    }
}