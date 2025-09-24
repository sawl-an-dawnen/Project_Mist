using UnityEngine;

public class GiveInversion : MonoBehaviour
{
    public bool lockInvertAbility = false;
    private GameManager gameManager;
    void Awake()
    {
        gameManager = GameManager.Instance;
    }

    // Called when another collider enters this trigger (2D only)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.SetCanInvert(true);
            if (lockInvertAbility)
            {
                gameManager.SetInvertAbilityLock(true);
            }
            Destroy(gameObject);
        }
    }
}
