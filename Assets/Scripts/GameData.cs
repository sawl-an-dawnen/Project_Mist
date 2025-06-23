using Unity.Profiling;

[System.Serializable]
public class GameData
{
    public int level;
    public float musicVolume;
    public float sfxVolume;

    public GameData(GameManager gameManager) {
        level = gameManager.GetLevel();
        musicVolume = gameManager.GetMusicVolume();
        sfxVolume = gameManager.GetSfxVolume();
    }
}
