using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [HideInInspector]
    public bool oneShot = false;
    public abstract void Interact();
    public abstract void Release();

}
public class Interact : MonoBehaviour
{
    public Transform interactPoint;
    public float interactRange = 3f;

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

            Collider2D[] hits = Physics2D.OverlapCircleAll(interactPoint.position, interactRange);

            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject.TryGetComponent(out Interactable obj) && !interacting)
                {
                    Action(obj);
                    break;
                }
            }
        }
    }

    private void Action(Interactable interactObj)
    {
        Debug.Log("Action called.");
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

    public Interactable GetInteraction() 
    {
        return interactable;
    }

    public bool Interacting() { return interacting; }

    void OnDrawGizmos()
    {
        // Visualize grab range in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(interactPoint.position, interactRange);
    }

}
