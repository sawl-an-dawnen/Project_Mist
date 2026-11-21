
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextScene : MonoBehaviour
{
    public string nextLevel;
    public bool activeOnNextScene = true;

    [Header("Audio Transition")]
    [SerializeField] private float audioFadeOutDuration = 2f;
    [SerializeField] private float audioFadeInDuration = 2f;

    [Header("Replace Music On Next Level")]
    [SerializeField] private bool replaceMusic = false;
    [SerializeField] private AudioClip trackAStart;
    [SerializeField] private AudioClip trackALoop;
    [SerializeField] private AudioClip trackBStart;
    [SerializeField] private AudioClip trackBLoop;

    [Header("Replace Ambiance On Next Level")]
    [SerializeField] private bool replaceAmbiance = false;
    [SerializeField] private AudioClip ambiance1;
    [SerializeField] private AudioClip ambiance2;

    private GameManager gameManager;
    private SceneController sceneController;
    private SpriteRenderer backupBlack;

    private bool transitioning = false;

    void Awake()
    {
        gameManager = GameManager.Instance;
        GameObject temp = GameObject.FindWithTag("GameController");
        sceneController = temp.GetComponent<SceneController>();
        backupBlack = GameObject.FindWithTag("UI_Backup_Black").GetComponent<SpriteRenderer>();
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        Debug.Log($"Loading scene async: {sceneName}");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        if (asyncLoad == null)
        {
            Debug.LogError("LoadSceneAsync returned null! Check scene name or build settings.");
            yield break;
        }

        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            Debug.Log($"Loading progress: {asyncLoad.progress}, allowSceneActivation: {asyncLoad.allowSceneActivation}");

            if (asyncLoad.progress >= 0.9f)
            {
                Debug.Log("Scene loaded to 90%, activating...");
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        Debug.Log("Scene activated successfully.");
    }

    public IEnumerator TransitionToNextScene()
    {
        if (transitioning)
            yield break;

        transitioning = true;

        AudioVolumeController audio = AudioVolumeController.Instance;

        // ---------------------------------------
        // PREPARE AUDIO FOR NEXT SCENE
        // ---------------------------------------

        if (audio != null)
        {
            // Store the next level's audio settings.
            // AudioVolumeController will apply them
            // after the new scene loads.
            audio.QueueNextSceneAudio(
                nextLevel,
                replaceMusic,
                trackAStart,
                trackALoop,
                trackBStart,
                trackBLoop,
                replaceAmbiance,
                ambiance1,
                ambiance2,
                audioFadeInDuration
            );

            // Always fade out all SFX.
            // Includes ambiance, footsteps, and
            // any other registered SFX sources.
            audio.FadeOutSfx(audioFadeOutDuration);

            // Only fade out music if we're
            // replacing the existing tracks.
            if (replaceMusic)
            {
                audio.FadeOutMusic(audioFadeOutDuration);
            }

            // Wait until the audio finishes fading out.
            yield return new WaitForSecondsRealtime(
                Mathf.Max(0f, audioFadeOutDuration)
            );
        }

        // ---------------------------------------
        // ORIGINAL SCENE TRANSITION LOGIC
        // ---------------------------------------

        gameManager.SetActiveOnNextRespawn(activeOnNextScene);

        yield return StartCoroutine(
            sceneController.PrepNextScene(nextLevel)
        );

        backupBlack.enabled = true;

        gameManager.SetLevel(nextLevel);

        yield return LoadSceneAsync(nextLevel);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !transitioning)
        {
            StartCoroutine(TransitionToNextScene());
        }
    }
}
