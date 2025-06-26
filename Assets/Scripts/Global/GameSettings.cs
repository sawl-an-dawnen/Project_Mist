using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings
{
    static GameSettings gameSettings = null;
    private float musicVolume = 50f;
    private float sfxVolume = 50f;

    public static GameSettings Instance
    {
        get
        {
            if (gameSettings == null)
            {
                gameSettings = new GameSettings();
            }

            return gameSettings;
        }
    }

    //I have not found a use for this function yet
    public void SetGameSettings(GameSettings gameSettings) { 
        musicVolume = gameSettings.musicVolume;
        sfxVolume = gameSettings.sfxVolume;
    }

    public float GetMusicVolume() { return musicVolume; }
    public void SetMusicVolume(float volume) { musicVolume = volume; }
    public float GetSfxVolume() { return sfxVolume; }
    public void SetSfxVolume(float volume) { sfxVolume = volume; }
}
