using System.Collections;
using UnityEngine;

public class ColorInverter : MonoBehaviour, Invertable
{
    public Color color01 = Color.black;
    public Color color02 = Color.white;
    private float transitionSpeed = 3f;
    private SpriteRenderer sprite;
    private bool inverted = false;


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

    private IEnumerator SmoothInvert(Color targetColor)
    {
        while (sprite.color != targetColor && sprite != null)
        {
            sprite.color = Color.Lerp(sprite.color, targetColor, Time.deltaTime * transitionSpeed);
            yield return null;
        }
    }
}
