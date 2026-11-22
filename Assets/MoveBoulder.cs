using UnityEngine;

public class MoveBoulder : MonoBehaviour
{
    [Header("Boulder Settings")]
    public GameObject boulder;
    public Transform targetLocation;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (boulder == null || targetLocation == null)
        {
            Debug.LogWarning("[MoveBoulder] Boulder or target not assigned.");
            return;
        }

        // Teleport boulder instantly
        boulder.transform.position = targetLocation.position;

        // Reset physics movement
        Rigidbody2D rb = boulder.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        Debug.Log("[MoveBoulder] Boulder teleported to respawn location.");
    }
}