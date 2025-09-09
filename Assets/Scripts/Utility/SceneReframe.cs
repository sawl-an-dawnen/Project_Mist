
using UnityEngine;
using Cinemachine;

public class SceneReframe : MonoBehaviour
{
    private Transform cameraPositionTransform;
    public Transform newCameraPositionTransform;
    public Transform temp;
    private CinemachineVirtualCamera m_Camera;

    private bool active = false;
    private bool transitionIn;

    public float newOrtho;
    public float transitionSpeed = 1f;

    // Start is called before the first frame update
    void Awake()
    {
        m_Camera = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        cameraPositionTransform = GameObject.FindGameObjectWithTag("CameraPosition").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraPositionTransform != null)
        {
            if (active && transitionIn)
            {
                temp.position = Vector3.Lerp(temp.position, newCameraPositionTransform.position, Time.deltaTime * transitionSpeed);
                if (Vector3.Distance(temp.position, newCameraPositionTransform.position) < .1f)
                {
                    m_Camera.Follow = newCameraPositionTransform;
                    active = false;
                }
            }
            if (active && !transitionIn)
            {
                temp.position = Vector3.Lerp(temp.position, cameraPositionTransform.position, Time.deltaTime * transitionSpeed);
                if (Vector3.Distance(temp.position, cameraPositionTransform.position) < 1f)
                {
                    m_Camera.Follow = cameraPositionTransform;
                    active = false;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player") 
        {
            if (!active) 
            {
                temp.position = cameraPositionTransform.position;
            }
            m_Camera.Follow = temp;
            active = true;
            transitionIn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (!active) 
            {
                temp.position = newCameraPositionTransform.position;
            }
            m_Camera.Follow = temp;
            active = true;
            transitionIn = false;
            //Debug.Log(transitionIn);
        }
    }
}
