using UnityEngine;

public class ContinuousSpin : MonoBehaviour
{
    public float rotationSpeed = 360f; // degrees per second

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
