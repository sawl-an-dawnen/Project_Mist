using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public bool oneShot;
    public abstract void Interact();
    public abstract void Release();

}
public class Interact : MonoBehaviour
{
    public Transform interactPoint;
    public float interactRange = 3f;
    public LayerMask interactionLayer;

    private bool interacting = false;
    private Interactable interactable;

    private void Awake() { }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Grab/Release toggle
        {

            if (interacting) 
            { 
                CounterAction(interactable);
                return;
            }

            Collider2D[] hits = Physics2D.OverlapCircleAll(interactPoint.position, interactRange, interactionLayer);

            if (hits[0].gameObject.TryGetComponent(out Interactable obj) && !interacting)
            {
                Action(obj);
            }
        }
    }

    private void Action(Interactable interactObj)
    {
        Debug.Log("Interaction called.");
        if (!interactObj.oneShot) 
        {
            interacting = true;
            interactable = interactObj;
        }
        //Debug.Log("INTERACT");
        interactObj.Interact();
    }

    private void CounterAction(Interactable interactObj) 
    {
        Debug.Log("Counter Action Called.");
        interacting = false;
        interactable = null;
        interactObj.Release();
    }

    void OnDrawGizmos()
    {
        // Visualize grab range in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(interactPoint.position, interactRange);
    }

}
