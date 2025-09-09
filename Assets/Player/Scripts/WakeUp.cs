using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WakeUp : MonoBehaviour
{
    public GameObject realPlayer;
    private Animator animator;

    private GameManager gameManager = GameManager.Instance;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
       realPlayer.SetActive(false);
    }

    public void TriggerWakeUp() {
        animator.SetBool("WakeUp", true);
    }

    public void OnAnimationEnd() { 
        realPlayer.SetActive(true);
        gameManager.SetInControl(true);
        Destroy(gameObject);
    }
}
