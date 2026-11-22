using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerEnding : MonoBehaviour
{
    public string sceneName;

    private bool triggered = false;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (triggered)
            return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            Debug.Log("Player entered the trigger area. Loading scene: " + sceneName);

            CleanupDontDestroy();

            SceneManager.LoadScene(sceneName);
        }
    }

    private void CleanupDontDestroy()
    {
        Debug.Log("[TriggerEnding] Cleaning DontDestroyOnLoad objects...");

        // Create a temporary object in the DontDestroyOnLoad scene
        GameObject temp = new GameObject("DDOL_CleanupHelper");
        DontDestroyOnLoad(temp);

        // Get the special DontDestroyOnLoad scene
        Scene ddolScene = temp.scene;

        // Retrieve all root objects from that scene
        GameObject[] rootObjects = ddolScene.GetRootGameObjects();

        foreach (GameObject obj in rootObjects)
        {
            if (obj != temp)
            {
                Debug.Log("[TriggerEnding] Destroying: " + obj.name);
                Destroy(obj);
            }
        }

        // Destroy the temporary helper
        Destroy(temp);
    }
}