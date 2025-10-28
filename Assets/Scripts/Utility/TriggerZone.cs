using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    public GameObject[] activate;
    public GameObject[] deactivate;
    public bool playerTrigger = true;
    public bool keyTrigger = false;

    public void OnTriggerEnter2D(Collider2D other) 
    {
        if ((playerTrigger && other.tag == "Player") || (keyTrigger && other.tag == "TriggerKey")) 
        {
            foreach (GameObject obj in activate) 
            {
                obj.SetActive(true);
            }
            foreach (GameObject obj in deactivate)
            {
                obj.SetActive(false);
            }
        }
    }
}
