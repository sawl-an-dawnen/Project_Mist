using System.Collections;
using UnityEngine;

public class ActiveInverter : MonoBehaviour, Invertable
{
    public float transitionSpeed = 3f;
    public bool includeGrandchildren = true;
    public bool fullyDisableOnFadeOut = true;

    public void Invert()
    {
        var children = includeGrandchildren
            ? GetComponentsInChildren<Transform>(true)
            : GetDirectChildren(transform);

        foreach (Transform child in children)
        {
            if (child == transform) continue; // skip self

            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            if (sr == null) continue;

            if (child.gameObject.activeSelf)
            {
                // Fade out, then deactivate
                StartCoroutine(FadeAndDisable(sr, false));
            }
            else
            {
                // Activate, then fade in
                child.gameObject.SetActive(true);
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f); // start invisible
                StartCoroutine(FadeAndDisable(sr, true));
            }
        }
    }

    private IEnumerator FadeAndDisable(SpriteRenderer sprite, bool fadeIn)
    {
        if (sprite == null)
        {
            yield break;
        }

        //float targetAlpha = fadeIn ? 1f : 0f;
        Color targetColor = fadeIn ? new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f) : new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0f);

        while (sprite.color != targetColor)
        {
            sprite.color = Color.Lerp(sprite.color, targetColor, Time.deltaTime * transitionSpeed);
            yield return null;
        }

        // Snap to final alpha
        sprite.color = targetColor;

        if (!fadeIn && fullyDisableOnFadeOut)
        {
            sprite.gameObject.SetActive(false);
        }
    }

    private Transform[] GetDirectChildren(Transform parent)
    {
        Transform[] children = new Transform[parent.childCount];
        for (int i = 0; i < parent.childCount; i++)
        {
            children[i] = parent.GetChild(i);
        }
        return children;
    }
}
