using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseController : MonoBehaviour
{

    private GameObject pauseUI;
    private GameObject pauseMenu;
    private GameObject settingsMenu;
    private GameObject firstButton;
    private EventSystem eventSystem;

    static GameManager gameManager = GameManager.Instance;
    private bool isPaused = false;

    void Awake() { 
        pauseUI = GameObject.FindWithTag("UI_Pause");
        pauseMenu = GameObject.FindWithTag("UI_Pause_Menu");
        settingsMenu = GameObject.FindWithTag("UI_Settings");
        firstButton = GameObject.FindWithTag("UI_Resume_Button");
        eventSystem = GameObject.FindWithTag("EventSystem").GetComponent<EventSystem>();
        settingsMenu.SetActive(false);
        pauseUI.SetActive(false);
    }

    //pause game
    public void Pause()
    {
        eventSystem = GameObject.FindWithTag("EventSystem").GetComponent<EventSystem>();
        isPaused = true;
        gameManager.SetPause(isPaused);
        pauseUI.SetActive(isPaused);
        pauseMenu.SetActive(isPaused);
        settingsMenu.SetActive(false);
        eventSystem.SetSelectedGameObject(firstButton);
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
