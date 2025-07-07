using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour
{

    public GameObject pauseUI;
    public GameObject pauseMenu;
    public GameObject settingsMenu;

    static GameManager gameManager = GameManager.Instance;
    private bool isPaused = false;

    //pause game
    public void Pause()
    {
        isPaused = true;
        gameManager.SetPause(isPaused);
        pauseUI.SetActive(isPaused);
        pauseMenu.SetActive(isPaused);
        settingsMenu.SetActive(false);
        Time.timeScale = 0;
    }

    //unpause game
    public void Resume()
    {
        isPaused = false;
        gameManager.SetPause(isPaused);
        pauseUI.SetActive(isPaused);
        settingsMenu.SetActive(isPaused);
        Time.timeScale = 1;
    }

    public void OnPause()
    {
        //Debug.Log("Pause Pressed");
        if (gameManager.InControl())
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
        else {
            Debug.Log("PauseController: NOT IN CONTROL");
        }
    }
}
