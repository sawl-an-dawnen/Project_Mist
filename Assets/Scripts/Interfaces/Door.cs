using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IDoor
{
    public void OpenDoor();

    public void CloseDoor();

    public void InvertState();
}
