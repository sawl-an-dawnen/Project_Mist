using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class WaterReflection : MonoBehaviour
{
    [Header("Reflection")]
    [SerializeField] private float yOffset = -0.75f;
    [SerializeField] private float alpha = 0.18f;
    [SerializeField] private float verticalScale = 0.65f;
    [SerializeField] private int sortingOrderOffset = -1;

    [Header("Wobble")]
    [SerializeField] private float wobbleAmount = 0.03f;
    [SerializeField] private float wobbleSpeed = 1.5f;

    private SpriteRenderer source;
    private SpriteRenderer reflection;

    private Material reflectionMaterial;
    private Material runtimeMaterial;

    private void Awake()
    {
        source = GetComponent<SpriteRenderer>();

        // Load default reflection material
        reflectionMaterial =
            Resources.Load<Material>("ReflectionFade_Mat");

        // Create reflection object
        GameObject reflectionObject =
            new GameObject($"{gameObject.name}_Reflection");

        reflectionObject.transform.SetParent(transform);

        reflection = reflectionObject.AddComponent<SpriteRenderer>();

        // Initial setup
        reflection.sprite = source.sprite;

        reflection.sortingLayerID = source.sortingLayerID;

        reflection.sortingOrder =
            source.sortingOrder + sortingOrderOffset;

        // Create unique material instance
        if (reflectionMaterial != null)
        {
            runtimeMaterial =
                new Material(reflectionMaterial);

            reflection.material = runtimeMaterial;
        }
    }

    private void LateUpdate()
    {
        if (source == null || reflection == null)
            return;

        // Keep reflection synced with source sprite
        reflection.sprite = source.sprite;

        reflection.flipX = source.flipX;

        // Position reflection below object
        reflection.transform.localPosition =
            new Vector3(0f, yOffset, 0f);

        // Wobble effect
        float wobble =
            Mathf.Sin(Time.time * wobbleSpeed)
            * wobbleAmount;

        // Vertical flip + squash
        reflection.transform.localScale =
            new Vector3(
                1f + wobble,
                -verticalScale,
                1f
            );

        // Fade reflection
        Color c = source.color;
        c.a = alpha;

        reflection.color = c;

        // Update shader alpha
        if (runtimeMaterial != null)
        {
            runtimeMaterial.SetFloat("_Alpha", alpha);
        }
    }
}

