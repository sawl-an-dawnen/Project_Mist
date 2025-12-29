using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager
{
    private static GameManager gameManager;
    static readonly GameSettings gameSettings = GameSettings.Instance;

    private string level = null;
    private bool newSession = true;
    private bool activeOnNextRespawn = false;

    private bool canInvert = false;
    private bool invertAbilityLock = false;
    private bool inverted = false;

    private bool inControl = false;
    private bool paused = false;

    public static GameManager Instance
    {
        get
        {
            if (gameManager == null)
            {
                gameManager = new GameManager();
            }

            return gameManager;
        }
    }

    private GameManager() {
        paused = false;
        inControl = false;
        inverted = false;
    }

    public string GetLevel() {
        if (level == null) {
            return "01_Forest";
        }
        return level; }
    public void SetLevel(string name) {
        level = name;
        Save();
    }
    public bool IsNewSession() { return newSession; }
    public void SetNewSession(bool state) { newSession = state; }
    public bool ActiveOnNextRespawnCheck() { return activeOnNextRespawn; }
    public void SetActiveOnNextRespawn(bool state) { activeOnNextRespawn = state; }

    public bool Inverted() { return inverted; }
    public void Invert() { inverted = !inverted; }
    public bool CanInvert() { return canInvert; }
    public void SetCanInvert(bool state) { canInvert = state; }
    public bool InvertAbilityLock() { return invertAbilityLock; }
    public void SetInvertAbilityLock(bool state) { invertAbilityLock = state; }


    public bool InControl() { return inControl; }
    public void SetInControl(bool state) { inControl = state; }
    public bool Paused() { return paused; }
    public void SetPause(bool state) { paused = state; }



    //save game
    public void Save() {
        SaveSystem.SaveGame(this, gameSettings);
    }

    //load game
    public void Load() {
        GameData data = SaveSystem.LoadGame();
        newSession = true;
        inverted = false;
        paused = false;
        inControl = false;
        canInvert = false;
        invertAbilityLock = false;
        if (data != null)
        {
            level = data.level;
            gameSettings.SetMusicVolume(data.musicVolume);
            gameSettings.SetSfxVolume(data.sfxVolume);
        }
        else
        {
            level = "Forest";
            gameSettings.SetMusicVolume(50f);
            gameSettings.SetSfxVolume(50f);
        }
    }

    public void ResetManager() {
        paused = false;
        inverted = false;
        inControl = false;
        canInvert = invertAbilityLock;
    }

}
