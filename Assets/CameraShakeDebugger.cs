using UnityEngine;
using Cinemachine;

public class CameraShakeDebugger : MonoBehaviour
{
    public CinemachineImpulseSource impulseSource;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X)) // Debug trigger
        {
            impulseSource.GenerateImpulse();
        }
    }
}