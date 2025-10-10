using UnityEngine;

public class SpriteOscillator : MonoBehaviour
{
    public float swaySpeed = 2f;
    public float swayAmount = 0.1f;
    public float scaleAmount = 0.05f;

    private Vector3 initialPos;
    private Vector3 initialScale;

    void Start()
    {
        initialPos = transform.position;
        initialScale = transform.localScale;
    }

    void Update()
    {
        float sway = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
        float scale = 1 + Mathf.Cos(Time.time * swaySpeed * 0.5f) * scaleAmount;

        transform.position = initialPos + new Vector3(sway, 0, 0);
        transform.localScale = initialScale * scale;
    }
}
