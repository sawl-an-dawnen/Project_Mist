
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AmbianceTrigger : MonoBehaviour
{
    [Header("Ambiance Replacement")]
    [SerializeField] private bool replaceAmbiance1 = false;
    [SerializeField] private AudioClip newAmbiance1;

    [SerializeField] private bool replaceAmbiance2 = false;
    [SerializeField] private AudioClip newAmbiance2;

    [Header("Transition Settings")]
    [SerializeField] private float fadeOutDuration = 2f;
    [SerializeField] private float fadeInDuration = 2f;

    [Header("Trigger Settings")]
    [SerializeField] private bool triggerOnce = true;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (triggerOnce && triggered)
            return;

        if (AudioVolumeController.Instance == null)
            return;

        triggered = true;

        if (replaceAmbiance1)
        {
            AudioVolumeController.Instance.TransitionAmbiance1(
                newAmbiance1,
                fadeOutDuration,
                fadeInDuration
            );
        }

        if (replaceAmbiance2)
        {
            AudioVolumeController.Instance.TransitionAmbiance2(
                newAmbiance2,
                fadeOutDuration,
                fadeInDuration
            );
        }
    }
}
