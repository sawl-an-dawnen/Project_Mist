using UnityEngine;

public class EnterTriggerSFAP : MonoBehaviour
{
    [Header("Objects to Trigger")]
    public SpriteFadeInPulse[] targets;

    [Header("Optional: Limit trigger to specific tag")]
    public string triggeringTag = "Player"; // leave empty to trigger on any collider

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;

        if (string.IsNullOrEmpty(triggeringTag) || other.CompareTag(triggeringTag))
        {
            foreach (SpriteFadeInPulse sfip in targets)
            {
                if (sfip != null)
                {
                    sfip.Trigger();
                }
            }

            hasTriggered = true;
            Destroy(gameObject);
        }
    }
}
