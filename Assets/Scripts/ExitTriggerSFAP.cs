using UnityEngine;

public class ExitTriggerSFAP : MonoBehaviour
{
    [Header("Objects to Fade Out")]
    public SpriteFadeInPulse[] targets;

    [Header("Optional: Limit trigger to specific tag")]
    public string triggeringTag = "Player"; // Leave empty to trigger on any collider

    private bool hasTriggered = false;

    private void OnTriggerExit2D(Collider2D other)
    {
        if (hasTriggered) return;

        if (string.IsNullOrEmpty(triggeringTag) || other.CompareTag(triggeringTag))
        {
            Debug.Log("ExitTriggerSFAP: Triggering FadeAway on SpriteFadeInPulse array...");

            foreach (SpriteFadeInPulse sfip in targets)
            {
                if (sfip != null)
                {
                    sfip.FadeAway();
                }
            }

            hasTriggered = true;

            // Destroy this GameObject (or just the script if you prefer)
            Destroy(gameObject); // or Destroy(this);
        }
    }
}
