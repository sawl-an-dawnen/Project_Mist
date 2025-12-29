using UnityEngine;
using Cinemachine;

public class CameraTransitionController : MonoBehaviour
{
    [Header("Targets")]
    public Transform currentTarget;
    public float positionLerpSpeed = 3f;

    [Header("Ortho Size")]
    public CinemachineVirtualCamera cam;
    public float desiredOrtho = 2f;
    public float orthoLerpSpeed = 2f;

    [Header("Current Offset")]
    public Vector2 currentOffset = Vector2.zero; // currently applied offset

    [Header("Default Values")]
    public Transform defaultTarget;
    public float defaultPositionLerpSpeed = 3f;
    public float defaultOrtho = 2f;
    public float defaultOrthoLerpSpeed = 2f;
    public Vector2 defaultOffset = Vector2.zero; // default horizontal/vertical offset

    void Awake()
    {
        defaultTarget = GameObject.FindGameObjectWithTag("CameraPosition").transform;
        currentTarget = defaultTarget;
        currentOffset = defaultOffset;

        if (cam == null)
        {
            cam = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        }

        cam.Follow = this.transform;
        cam.LookAt = this.transform;
    }

    void LateUpdate()
    {
        if (currentTarget == null || cam == null) return;

        // 2D Camera Position Lerp with Offset
        Vector3 targetPos = currentTarget.position + new Vector3(currentOffset.x, currentOffset.y, 0f);
        targetPos.z = transform.position.z; // keep camera Z depth

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            Time.deltaTime * positionLerpSpeed
        );

        // Orthographic Zoom Lerp via Cinemachine
        cam.m_Lens.OrthographicSize = Mathf.Lerp(
            cam.m_Lens.OrthographicSize,
            desiredOrtho,
            Time.deltaTime * orthoLerpSpeed
        );
    }

    // Called by reframe zones to set new camera behavior
    public void SetNewTarget(Transform t, float posSpeed, Vector2 offset)
    {
        currentTarget = t;
        positionLerpSpeed = posSpeed;
        currentOffset = offset;
    }

    public void SetNewOrtho(float newSize, float lerpSpeed)
    {
        desiredOrtho = newSize;
        orthoLerpSpeed = lerpSpeed;
    }

    public void ResetToDefault()
    {
        currentTarget = defaultTarget;
        positionLerpSpeed = defaultPositionLerpSpeed;
        desiredOrtho = defaultOrtho;
        orthoLerpSpeed = defaultOrthoLerpSpeed;
        currentOffset = defaultOffset;
    }
}
