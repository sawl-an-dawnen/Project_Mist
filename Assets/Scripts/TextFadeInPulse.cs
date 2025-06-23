using System.Collections;
using TMPro;
using UnityEngine;

public class TextFadeInPulse : MonoBehaviour
{

    public float delayBeforeFade = 1f;
    public float fadeDuration = 1f;
    public float pulseSpeed = 1.5f;
    public float lowerLimit = 0f;
    public bool deleteOnFade = true;

    private TextMeshProUGUI text;
    private Color originalColor;
    private Vector3 originalScale;
    private float amplitude;
    private float offset;

    private UIController uiController;
    private bool pulsing = true;

    void Awake() {
        text = gameObject.GetComponent<TextMeshProUGUI>();
        originalColor = text.color;
        text.color = new Color(0f, 0f, 0f, 0f);
        uiController = GameObject.FindWithTag("GameController").GetComponent<UIController>();
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
    }

    public void Trigger() {
        // Start the coroutine
        Debug.Log("Trigger TFIP...");
        StartCoroutine(FadeInAndPulse());
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
        else {
            yield return null;
        }
    }

    public void FadeAway() {
        Debug.Log("TFIP: FadeAway()...");
        pulsing = false;
        uiController.FadeTMP(text, 0f, fadeDuration);
    }
}