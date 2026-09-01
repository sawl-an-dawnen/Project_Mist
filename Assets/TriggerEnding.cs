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
            //SceneManager.LoadScene(sceneName);
        }
    }
}