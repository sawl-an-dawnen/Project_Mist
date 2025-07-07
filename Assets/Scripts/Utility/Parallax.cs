using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Transform cameraTransform; // Reference to the camera
    public Vector2 parallaxEffectMultiplier; // Speed multiplier for the parallax effect
    private Vector3 lastCameraPosition; // Store the last position of the camera

    void Start()
    {
        // Store the initial camera position
        lastCameraPosition = cameraTransform.position;
    }

    void Update()
    {
        // Calculate the parallax effect based on the camera's movement
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxEffectMultiplier.x, deltaMovement.y * parallaxEffectMultiplier.y, 0);

        // Update the last camera position
        lastCameraPosition = cameraTransform.position;
    }
}