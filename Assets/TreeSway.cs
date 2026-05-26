using UnityEngine;

public class TreeSway : MonoBehaviour
{
    [Header("Wind Sway")]
    [SerializeField] private float swayAmount = 2f;
    [SerializeField] private float swaySpeed = 0.2f;
    [SerializeField] private float randomOffset = 100f;

    private float startZ;
    private float offset;

    private void Awake()
    {
        startZ = transform.localEulerAngles.z;
        offset = Random.Range(0f, randomOffset);
    }

    private void Update()
    {
        float sway = Mathf.Sin((Time.time + offset) * swaySpeed) * swayAmount;

        transform.localRotation = Quaternion.Euler(
            transform.localEulerAngles.x,
            transform.localEulerAngles.y,
            startZ + sway
        );
    }
}