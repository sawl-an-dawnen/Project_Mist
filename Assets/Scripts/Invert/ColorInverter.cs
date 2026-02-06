using System.Collections;
using UnityEngine;

public class ColorInverter : MonoBehaviour, IInvertable
{
    public Color color01 = Color.black;
    public Color color02 = Color.white;
    private float transitionSpeed = 3f;
    private SpriteRenderer sprite;
    private bool inverted = false;
    private bool active = true;

    // Start is called before the first frame update
    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = color01;
    }

    public void Invert() 
    {
        if (gameObject.activeSelf == true) {
            StopAllCoroutines(); // Stop any ongoing transitions
            StartCoroutine(SmoothInvert(inverted ? color01 : color02));
            inverted = !inverted;
        }
    }

    public void SetActive(bool isActive)
    {
        active = isActive;
    }

    private IEnumerator SmoothInvert(Color targetColor)
    {
        if (!active)
            yield break;
        while (sprite.color != targetColor && sprite != null)
        {
            sprite.color = Color.Lerp(sprite.color, targetColor, Time.deltaTime * transitionSpeed);
            yield return null;
        }
    }
}
