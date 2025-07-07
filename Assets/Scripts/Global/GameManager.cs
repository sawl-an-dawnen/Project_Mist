using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager
{
    private static GameManager gameManager;
    static readonly GameSettings gameSettings = GameSettings.Instance;
    private string level = null;
    private bool inverted = false;
    private bool paused = false;
    private bool inControl = false;

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
            return "Forest";
        }
        return level; }
    public void SetLevel(string name) {
        level = name;
        Save();
    }

    public bool Inverted() { return inverted; }
    public void Invert() { inverted = !inverted; }

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

    public void Reset() {
        paused = false;
        inverted = false;
        inControl = false;
    }

}
