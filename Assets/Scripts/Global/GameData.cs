using Unity.Profiling;

[System.Serializable]
public class GameData
{
    public string level;
    public float musicVolume;
    public float sfxVolume;

    public GameData(GameManager gameManager, GameSettings gameSettings) {
        level = gameManager.GetLevel();
        musicVolume = gameSettings.GetMusicVolume();
        sfxVolume = gameSettings.GetSfxVolume();
    }
}
