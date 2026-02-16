using UnityEngine;

public class BinaryDoorTrigger : MonoBehaviour
{
    //public IDoor[] doors;
    public GameObject[] doors;
    public bool closeDoorOnTrigger = false;
    public bool openDoorOnTrigger = false;
    public bool invertDoorOnTrigger = false;
    public BinaryDoorTrigger oppositeTrigger;
    public bool isTriggered = false;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TriggerKey") && !isTriggered)
        {
            foreach (GameObject door in doors)
            {
                IDoor doorComponent = door.GetComponent<IDoor>();
                if (closeDoorOnTrigger)
                {
                    doorComponent.CloseDoor();
                }
                if (openDoorOnTrigger)
                {
                    doorComponent.OpenDoor();
                }
                if (invertDoorOnTrigger)
                {
                    doorComponent.InvertState();
                }
            }
            isTriggered = true;
            oppositeTrigger?.ClearTrigger();
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("Touching: " + other.name);
    }

    public void ClearTrigger() { isTriggered = false; }
}
