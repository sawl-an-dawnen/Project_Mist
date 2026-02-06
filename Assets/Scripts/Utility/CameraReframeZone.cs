using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CameraReframeZone2D : MonoBehaviour
{
    [Header("Zone Settings")]
    public Transform targetToFollow;
    public float positionLerp = 3f;
    public float targetOrtho = 5f;
    public float orthoLerpSpeed = 2f;

    [Header("Zone Offset")]
    public Vector2 offset = Vector2.zero; // horizontal/vertical offset for this zone

    [Header("Sprite Fade")]
    public SpriteRenderer spriteToFade;
    public float fadeDuration = 1f;

    private CameraTransitionController camController;
    private Coroutine fadeCoroutine;
    private ColorInverter colorInverter;

    void Awake()
    {
        camController = FindObjectOfType<CameraTransitionController>();

        if (spriteToFade != null)
            colorInverter = spriteToFade.GetComponent<ColorInverter>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Pass the offset along with target and lerp speed
        camController.SetNewTarget(targetToFollow, positionLerp, offset);
        camController.SetNewOrtho(targetOrtho, orthoLerpSpeed);

        if (spriteToFade != null) 
        {
            StartFade(1f, 0f); // fade out
        }

        // Disable color inverter
        if (colorInverter != null)
            colorInverter.SetActive(false);

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Return to default values defined in CameraTransitionController
        camController.ResetToDefault();

        if (spriteToFade != null)
        {
            StartFade(0f, 1f); // fade back in (optional)
        }

        // Disable color inverter
        if (colorInverter != null)
            colorInverter.SetActive(true);
    }

    private void StartFade(float from, float to)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeSprite(from, to));
    }

    private IEnumerator FadeSprite(float startAlpha, float endAlpha)
    {
        Color color = spriteToFade.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            spriteToFade.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        spriteToFade.color = new Color(color.r, color.g, color.b, endAlpha);
        Destroy(spriteToFade.gameObject);
    }
}
