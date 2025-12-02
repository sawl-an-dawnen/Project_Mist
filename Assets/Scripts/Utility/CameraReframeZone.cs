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

    private CameraTransitionController camController;

    void Start()
    {
        camController = FindObjectOfType<CameraTransitionController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Pass the offset along with target and lerp speed
        camController.SetNewTarget(targetToFollow, positionLerp, offset);
        camController.SetNewOrtho(targetOrtho, orthoLerpSpeed);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Return to default values defined in CameraTransitionController
        camController.ResetToDefault();
    }
}
