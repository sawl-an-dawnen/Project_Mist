using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorInvert_Particle : MonoBehaviour, Invertable
{
    public Color color01;
    public Color color02;
    private float transitionSpeed = 3f;
    private ParticleSystem particles;
    private bool inverted = false;

    // Start is called before the first frame update
    void Awake()
    {
        particles = GetComponent<ParticleSystem>();
        particles.startColor = color01;
    }

    public void Invert()
    {
        /*
        if (inverted)
        {
            sprite.color = color01;
        }
        else
        {
            sprite.color = color02;
        }
        inverted = !inverted;
        */

        StopAllCoroutines(); // Stop any ongoing transitions
        StartCoroutine(SmoothInvertParticle(inverted ? color01 : color02));
        inverted = !inverted;
    }

    private IEnumerator SmoothInvertParticle(Color targetColor)
    {
        while (particles.startColor != targetColor)
        {
                particles.startColor = Color.Lerp(particles.startColor, targetColor, Time.deltaTime * transitionSpeed);
            yield return null;
        }
    }
}
