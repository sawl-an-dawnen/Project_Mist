using Cinemachine;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    private Transform defaultSpawn;
    private Transform spawn = null;
    private Transform player;
    private SceneController sceneController;

    private CinemachineVirtualCamera cam;
    private float defaultOrthographicSize = 2f;
    private Transform defaultCameraTarget;

    // Start is called before the first frame update
    void Awake()
    {
        sceneController = GameObject.FindWithTag("GameController").GetComponent<SceneController>();
        player = GameObject.FindWithTag("Player_wrapper").transform;
        defaultSpawn = GameObject.FindWithTag("Default_Spawn").transform;
        cam = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        defaultCameraTarget = GameObject.FindWithTag("CameraPosition").GetComponent<Transform>();
    }

    void Start() {
        cam.m_Lens.OrthographicSize = defaultOrthographicSize;
        GameObject tempSpawn = sceneController.GetCheckpoint();

        if (tempSpawn != null) {
            spawn = tempSpawn.transform;
        }
        if (spawn == null) {
            spawn = defaultSpawn;
        }

        player.position = spawn.position;
    }
    public void ResetController()
    {
        sceneController = GameObject.FindWithTag("GameController").GetComponent<SceneController>();
        player = GameObject.FindWithTag("Player_wrapper").transform;
        defaultSpawn = GameObject.FindWithTag("Default_Spawn").transform;
    }

    public void Spawn() {
        try {
            GameObject tempSpawn = sceneController.GetCheckpoint();
            spawn = tempSpawn.transform;
        }
        catch {
            spawn = defaultSpawn;
        }

        player.position = spawn.position;
    }
}
