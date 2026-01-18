using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorInverterRawImage : MonoBehaviour, IInvertable
{
    public Color color01 = Color.black;
    public Color color02 = Color.white;
    private float transitionSpeed = 3f;
    private RawImage image;
    private bool inverted = false;


    // Start is called before the first frame update
    void Awake()
    {
        image = GetComponent<RawImage>();
        image.color = color01;
    }

    public void Invert()
    {
        StopAllCoroutines(); // Stop any ongoing transitions
        StartCoroutine(SmoothInvert(inverted ? color01 : color02));
        inverted = !inverted;
    }

    private IEnumerator SmoothInvert(Color targetColor)
    {
        while (image.color != targetColor && image != null)
        {
            image.color = Color.Lerp(image.color, targetColor, Time.deltaTime * transitionSpeed);
            yield return null;
        }
    }
}
