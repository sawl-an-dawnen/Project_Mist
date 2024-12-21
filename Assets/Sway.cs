using UnityEngine;

public class Sway : MonoBehaviour
{
    public float swaySpeed = 2f; // Speed of swaying
    public float swayAmount = 15f; // Amount of rotation in degrees

    private float startRotationZ;

    void Start()
    {
        startRotationZ = transform.eulerAngles.z;
    }

    void Update()
    {
        float sway = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
        transform.rotation = Quaternion.Euler(0, 0, startRotationZ + sway);
    }
}