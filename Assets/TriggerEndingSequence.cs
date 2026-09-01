using UnityEngine;

public class TriggerEndingSequence : MonoBehaviour
{
    public GameObject objectToMove;
    public GameObject objectToActivate;
    public GameObject objectToDelete;

    public float moveDistance = 5f;
    public float moveSpeed = 10f;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered)
            return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            // Activate the dormant object
            objectToActivate.SetActive(true);

            // Move both objects left quickly
            objectToMove.transform.position += Vector3.left * moveDistance;
            objectToActivate.transform.position += Vector3.left * moveDistance;

            // Delete the other object
            Destroy(objectToDelete);
        }
    }
}