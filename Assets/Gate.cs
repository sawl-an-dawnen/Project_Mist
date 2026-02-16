using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Gate : MonoBehaviour, IDoor
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
        DoorAnimations.Play("GateOpenAnimation");
    }
    public void CloseDoor()
    {
        return;
    }

    public void InvertState()
    {
        if (!isOpen)
        {
            OpenDoor();
        }
    }

    public bool IsDoorOpen()
    {
        return isOpen;
    }
}
