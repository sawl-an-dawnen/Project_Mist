using System.Collections;
using UnityEngine;

public class SpriteFadeInPulse : MonoBehaviour
{
    [Header("Timing and Triggering")]
    public bool triggerOnStart = true;
    public float delayBeforeFade = 1f;
    public float fadeInDuration = 1f;
    public float fadeOutDuration = 1f;

    [Header("Pulse Settings")]
    public float pulseSpeed = 1.5f;
    public float lowerLimit = 0f;

    [Header("Behavior")]
    public bool deleteOnFade = true;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool pulsing = false;
    private float amplitude;
    private float offset;
    private bool fading = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on this GameObject.");
            enabled = false;
            return;
        }

        originalColor = spriteRenderer.color;
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f); // start transparent

        amplitude = (1f - lowerLimit) / 2f;
        offset = lowerLimit + amplitude;
    }

    void Start()
    {
        if (triggerOnStart)
        {
            Trigger(); // Automatically start
        }
    }

    /// <summary>
    /// Manually trigger fade-in and pulsing effect.
    /// </summary>
    public void Trigger()
    {
        if (!pulsing)
        {
            pulsing = true;
            StartCoroutine(FadeInAndPulse());
        }
    }

    /// <summary>
    /// Stops pulsing and begins fade-out. Optionally destroys the GameObject.
    /// </summary>
    public void FadeAway()
    {
        fading = true;
        if (pulsing)
        {
            pulsing = false;
            StartCoroutine(FadeOut());
        }
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeInAndPulse()
    {
        if (!spriteRenderer) yield break;

        // Optional delay before fade-in
        yield return new WaitForSeconds(delayBeforeFade);

            // Fade in
            float elapsed = 0f;
            while (elapsed < fadeInDuration && !fading)
            {
                float alpha = Mathf.Lerp(0f, originalColor.a, elapsed / fadeInDuration);
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                elapsed += Time.deltaTime;
                yield return null;
            }

            spriteRenderer.color = originalColor;

            // Start pulsing
            float time = 0f;
            while (pulsing)
            {
                float pulse = offset + Mathf.Sin(time * pulseSpeed + Mathf.PI / 2f) * amplitude;
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, pulse);
                time += Time.deltaTime;
                yield return null;
            }
    }

    IEnumerator FadeOut()
    {
        if (!spriteRenderer) yield break;

        Color currentColor = spriteRenderer.color;
        float startAlpha = currentColor.a;
        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeOutDuration);
            spriteRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0f);

        if (deleteOnFade && spriteRenderer.color.a <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
