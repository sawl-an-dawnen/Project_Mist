using UnityEngine;

public class SlidingDoor : MonoBehaviour, IDoor
{

    public bool isOpen = false;
    private Animation DoorAnimations;

    void Start()
    {
        DoorAnimations = GetComponent<Animation>();
        if (isOpen) 
        {
            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        isOpen = true;
        DoorAnimations.Play("SlidingDoorOpenAnimation");
    }
    public void CloseDoor()
    {
        isOpen = false;
        DoorAnimations.Play("SlidingDoorClosingAnimation");
    }

    public void InvertState()
    {
        if (isOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }
    }

    public bool IsDoorOpen()
    {
        return isOpen;
    }
}
