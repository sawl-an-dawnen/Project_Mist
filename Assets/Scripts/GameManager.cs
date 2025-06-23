using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private int level;
    private GameObject checkpoint;
    private GameObject player;
    private bool alive;
    private bool inControl = false;
    private float musicVolume;
    private float sfxVolume;

    //UI
    private GameObject titleScreen;
    private GameObject pauseMenu;
    private GameObject settingsMenu;
    private RawImage blackScreen;

    private Coroutine currentTransition;
    private float duration = 1f;

    private bool gameStarted = false;

    void Awake() {
        titleScreen = GameObject.FindWithTag("UI_Title");
        pauseMenu = GameObject.FindWithTag("UI_Pause");
        settingsMenu = GameObject.FindWithTag("UI_Settings");
        blackScreen = GameObject.FindWithTag("UI_Black").GetComponent<RawImage>();
        //Load();
        //spawn player in designated spot
    }

    public int GetLevel() { return level; }
    public void SetLevel(int i) { 
        level = i;
        Save();
    }

    public GameObject GetCheckpoint() { return checkpoint; }
    public void SetCheckpoint(GameObject checkpoint)
    {
        this.checkpoint = checkpoint;
        Save();
    }

    public GameObject GetPlayer() { return player; }
    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }

    public bool IsAlive() { return alive; }
    public void SetAlive(bool state) { alive = state; }

    public bool InControl() { return inControl; }
    public void SetInControl(bool state) { inControl = state; }


    public float GetMusicVolume() { return musicVolume; }
    public void SetMusicVolume(float volume) { musicVolume = volume; }
    public float GetSfxVolume() { return sfxVolume; }
    public void SetSfxVolume(float volume) { sfxVolume = volume; }




    //start game

    public void StartGame() { 
        //fade out menu
        //trigger wake up -> should release control to player here
        //give control to player
    }

    //end game
    public void EndGame() { 
        //take control from player
        //fade out
        //close game
    }

    //pause game
    public void Pause() {
        if (inControl) { }
        //take control away
        //turn on pause screen
    }

    //unpause game
    public void Resume() {
        //turn pause screen off
        //return control
    }

    //reset game
    public void Reset() {
        //delete player
        //say player is alive
        alive = true;
        //instantiate new player
        //load check point -- move player to new position
        player.transform.position = this.checkpoint.transform.position;

    }

    //save game
    public void Save() {
        SaveSystem.SaveGame(this);
    }

    //load game
    public void Load() {
        GameData data = SaveSystem.LoadGame();
        if (data != null)
        {
            level = data.level;
            musicVolume = data.musicVolume;
            sfxVolume = data.sfxVolume;
        }
        else
        {
            level = 0;
            musicVolume = 50f;
            sfxVolume = 50f;
        }
    }

    private IEnumerator LoadCheckPoint(Color targetColor)
    {
        yield return StartCoroutine(FadeOutScene());
        Reset();
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FadeInScene());
    }

    //fade in
    public IEnumerator FadeInScene() {
        if (currentTransition != null) { 
            StopCoroutine(currentTransition);
        }

        currentTransition = StartCoroutine(TransitionColorCoroutine(new Color(0f,0f,0f,0f)));
        yield return null;
    }
    //fade out
    public IEnumerator FadeOutScene() {
        if (currentTransition != null)
        {
            StopCoroutine(currentTransition);
        }

        currentTransition = StartCoroutine(TransitionColorCoroutine(new Color(0f,0f,0f,1f)));
        yield return null;
    }

    private IEnumerator TransitionColorCoroutine(Color targetColor)
    {
        Color startColor = blackScreen.color;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            blackScreen.color = Color.Lerp(startColor, targetColor, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        blackScreen.color = targetColor; // Ensure it finishes exactly at target
        currentTransition = null;
    }
}
