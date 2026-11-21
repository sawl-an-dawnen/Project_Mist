
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioVolumeController : MonoBehaviour
{
    public static AudioVolumeController Instance;

    [Header("Music Sources - Track A")]
    [SerializeField] private AudioSource trackAStart;
    [SerializeField] private AudioSource trackALoop;

    [Header("Music Sources - Track B")]
    [SerializeField] private AudioSource trackBStart;
    [SerializeField] private AudioSource trackBLoop;

    [Header("Music Settings")]
    [SerializeField] private bool playMusicOnStart = true;
    [SerializeField] private float defaultCrossfadeDuration = 2f;
    [SerializeField] private float loopStartOffset = 0f;

    [Header("Ambiance Sources")]
    [SerializeField] private AudioSource ambiance1;
    [SerializeField] private AudioSource ambiance2;

    [Header("Ambiance Settings")]
    [SerializeField] private float ambianceStartFadeDuration = 3f;

    [Header("Additional SFX Objects")]
    [SerializeField] private GameObject[] sfxObjects;

    private Slider music_slider;
    private Slider sfx_slider;

    private AudioSource footsteps;
    private GameSettings gameSettings;

    private Dictionary<AudioSource, float> baseVolumes =
        new Dictionary<AudioSource, float>();

    private List<AudioSource> sfxSources =
        new List<AudioSource>();

    private Dictionary<AudioSource, float> ambianceFades =
        new Dictionary<AudioSource, float>();

    private Dictionary<AudioSource, Coroutine> ambianceRoutines =
        new Dictionary<AudioSource, Coroutine>();

    // 0 = Track A, 1 = Track B
    private float musicBlend = 0f;

    // Overall category fade multipliers
    private float musicFade = 1f;
    private float sfxFade = 1f;

    private Coroutine crossfadeRoutine;
    private Coroutine musicFadeRoutine;
    private Coroutine sfxFadeRoutine;

    // ---------------------------------------
    // PENDING SCENE AUDIO SETTINGS
    // ---------------------------------------

    private bool pendingSceneAudio = false;
    private string pendingSceneName;

    private bool pendingReplaceMusic;
    private bool pendingReplaceAmbiance;

    private AudioClip pendingTrackAStart;
    private AudioClip pendingTrackALoop;
    private AudioClip pendingTrackBStart;
    private AudioClip pendingTrackBLoop;

    private AudioClip pendingAmbiance1;
    private AudioClip pendingAmbiance2;

    private float pendingAudioFadeInDuration;

    // ---------------------------------------
    // INITIALIZATION
    // ---------------------------------------

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        gameSettings = GameSettings.Instance;

        // Register music sources
        RegisterSource(trackAStart);
        RegisterSource(trackALoop);
        RegisterSource(trackBStart);
        RegisterSource(trackBLoop);

        // Register ambiance as SFX
        RegisterSfx(ambiance1);
        RegisterSfx(ambiance2);

        // Register additional SFX sources
        foreach (GameObject obj in sfxObjects)
        {
            RegisterSfxObject(obj);
        }

        // Configure music looping
        if (trackAStart != null)
            trackAStart.loop = false;

        if (trackBStart != null)
            trackBStart.loop = false;

        if (trackALoop != null)
            trackALoop.loop = true;

        if (trackBLoop != null)
            trackBLoop.loop = true;

        SceneManager.sceneLoaded += OnSceneLoaded;

        ApplyVolumes();
    }

    void Start()
    {
        RefreshSceneReferences();

        if (playMusicOnStart)
            PlayMusic();

        // Automatically fade in ambiance 1
        if (ambiance1 != null && ambiance1.clip != null)
        {
            ambianceFades[ambiance1] = 0f;
            ApplyVolumes();

            FadeInAmbiance1(ambianceStartFadeDuration);
        }

        // Automatically fade in ambiance 2
        if (ambiance2 != null && ambiance2.clip != null)
        {
            ambianceFades[ambiance2] = 0f;
            ApplyVolumes();

            FadeInAmbiance2(ambianceStartFadeDuration);
        }
    }

    void Update()
    {
        // Reconnect sliders when they become available
        if (music_slider == null)
            FindMusicSlider();

        if (sfx_slider == null)
            FindSfxSlider();

        // Reconnect footsteps if the player respawns
        if (footsteps == null)
            FindFootsteps();
    }

    // ---------------------------------------
    // SCENE MANAGEMENT
    // ---------------------------------------

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshSceneReferences();

        // Complete audio transition only for
        // the scene we are expecting
        if (pendingSceneAudio && scene.name == pendingSceneName)
        {
            StartCoroutine(CompleteSceneAudioTransition());
        }
    }

    private void RefreshSceneReferences()
    {
        // Disconnect previous slider listeners
        if (music_slider != null)
            music_slider.onValueChanged.RemoveListener(SetMusicVolume);

        if (sfx_slider != null)
            sfx_slider.onValueChanged.RemoveListener(SetSfxVolume);

        music_slider = null;
        sfx_slider = null;

        // Remove previous level's footsteps
        if (footsteps != null)
            sfxSources.Remove(footsteps);

        footsteps = null;

        // Find objects in new level
        FindMusicSlider();
        FindSfxSlider();
        FindFootsteps();

        ApplyVolumes();
    }

    // ---------------------------------------
    // FIND SLIDERS BY TAG
    // ---------------------------------------

    private void FindMusicSlider()
    {
        GameObject obj = GameObject.FindWithTag("music_slider");

        if (obj == null)
            return;

        Slider slider = obj.GetComponent<Slider>();

        if (slider == null)
            return;

        music_slider = slider;

        // Restore position before attaching listener
        music_slider.value = gameSettings.GetMusicVolume();

        music_slider.onValueChanged.AddListener(SetMusicVolume);

        ApplyVolumes();

        Debug.Log("Music slider connected.");
    }

    private void FindSfxSlider()
    {
        GameObject obj = GameObject.FindWithTag("sfx_slider");

        if (obj == null)
            return;

        Slider slider = obj.GetComponent<Slider>();

        if (slider == null)
            return;

        sfx_slider = slider;

        // Restore position before attaching listener
        sfx_slider.value = gameSettings.GetSfxVolume();

        sfx_slider.onValueChanged.AddListener(SetSfxVolume);

        ApplyVolumes();

        Debug.Log("SFX slider connected.");
    }

    // ---------------------------------------
    // FIND FOOTSTEPS BY TAG
    // ---------------------------------------

    private void FindFootsteps()
    {
        GameObject obj = GameObject.FindWithTag("Player_Footsteps");

        if (obj == null)
            return;

        AudioSource source = obj.GetComponent<AudioSource>();

        if (source == null)
            source = obj.GetComponentInChildren<AudioSource>();

        if (source == null)
            return;

        footsteps = source;

        RegisterSfx(footsteps);

        ApplyVolumes();
    }

    // ---------------------------------------
    // AUDIO SOURCE REGISTRATION
    // ---------------------------------------

    private void RegisterSource(AudioSource source)
    {
        if (source == null)
            return;

        if (!baseVolumes.ContainsKey(source))
        {
            baseVolumes.Add(source, source.volume);
        }
    }

    private void RegisterSfx(AudioSource source)
    {
        if (source == null)
            return;

        RegisterSource(source);

        if (!sfxSources.Contains(source))
            sfxSources.Add(source);
    }

    public void RegisterSfxObject(GameObject obj)
    {
        if (obj == null)
            return;

        AudioSource[] sources =
            obj.GetComponentsInChildren<AudioSource>(true);

        foreach (AudioSource source in sources)
        {
            RegisterSfx(source);
        }

        ApplyVolumes();
    }

    // ---------------------------------------
    // MASTER VOLUME CONTROL
    // ---------------------------------------

    public void SetMusicVolume(float volume)
    {
        gameSettings.SetMusicVolume(
            Mathf.Clamp(volume, 0f, 100f)
        );

        ApplyVolumes();
    }

    public void SetSfxVolume(float volume)
    {
        gameSettings.SetSfxVolume(
            Mathf.Clamp(volume, 0f, 100f)
        );

        ApplyVolumes();
    }

    // ---------------------------------------
    // APPLY ALL AUDIO VOLUMES
    // ---------------------------------------

    private void ApplyVolumes()
    {
        // Master volume ratios
        float musicMultiplier =
            Mathf.Clamp01(gameSettings.GetMusicVolume());

        float sfxMultiplier =
            Mathf.Clamp01(gameSettings.GetSfxVolume());

        // Apply overall category fades
        musicMultiplier *= musicFade;
        sfxMultiplier *= sfxFade;

        // Apply music crossfade ratio
        float trackAVolume =
            musicMultiplier * (1f - musicBlend);

        float trackBVolume =
            musicMultiplier * musicBlend;

        // Track A
        SetSourceVolume(trackAStart, trackAVolume);
        SetSourceVolume(trackALoop, trackAVolume);

        // Track B
        SetSourceVolume(trackBStart, trackBVolume);
        SetSourceVolume(trackBLoop, trackBVolume);

        // All SFX, including ambiance and footsteps
        foreach (AudioSource source in sfxSources)
        {
            if (source == null)
                continue;

            float fade = 1f;

            if (ambianceFades.ContainsKey(source))
                fade = ambianceFades[source];

            SetSourceVolume(source, sfxMultiplier * fade);
        }
    }

    private void SetSourceVolume(AudioSource source, float multiplier)
    {
        if (source == null)
            return;

        if (!baseVolumes.ContainsKey(source))
            RegisterSource(source);

        source.volume =
            baseVolumes[source] * Mathf.Clamp01(multiplier);
    }

    // ---------------------------------------
    // MUSIC PLAYBACK
    // ---------------------------------------

    public void PlayMusic()
    {
        StopMusic();

        if (trackAStart == null || trackALoop == null ||
            trackBStart == null || trackBLoop == null)
            return;

        if (trackAStart.clip == null || trackALoop.clip == null ||
            trackBStart.clip == null || trackBLoop.clip == null)
            return;

        double aDuration =
            (double)trackAStart.clip.samples /
            trackAStart.clip.frequency;

        double bDuration =
            (double)trackBStart.clip.samples /
            trackBStart.clip.frequency;

        if (System.Math.Abs(aDuration - bDuration) > 0.001)
        {
            Debug.LogWarning(
                "Track A and B start clips have different lengths!"
            );
        }

        double aLoopDuration =
            (double)trackALoop.clip.samples /
            trackALoop.clip.frequency;

        double bLoopDuration =
            (double)trackBLoop.clip.samples /
            trackBLoop.clip.frequency;

        if (System.Math.Abs(aLoopDuration - bLoopDuration) > 0.001)
        {
            Debug.LogWarning(
                "Track A and B loop clips have different lengths!"
            );
        }

        trackAStart.loop = false;
        trackBStart.loop = false;

        trackALoop.loop = true;
        trackBLoop.loop = true;

        // Schedule both versions using DSP timing
        double startTime = AudioSettings.dspTime + 0.2;

        double loopTime =
            startTime + aDuration + loopStartOffset;

        ApplyVolumes();

        trackAStart.PlayScheduled(startTime);
        trackBStart.PlayScheduled(startTime);

        trackALoop.PlayScheduled(loopTime);
        trackBLoop.PlayScheduled(loopTime);
    }

    public void StopMusic()
    {
        if (trackAStart != null)
            trackAStart.Stop();

        if (trackALoop != null)
            trackALoop.Stop();

        if (trackBStart != null)
            trackBStart.Stop();

        if (trackBLoop != null)
            trackBLoop.Stop();
    }

    // ---------------------------------------
    // MUSIC CLIP MANAGEMENT
    // ---------------------------------------

    public void SetTrackA(AudioClip startClip, AudioClip loopClip)
    {
        StopMusic();

        if (trackAStart != null)
            trackAStart.clip = startClip;

        if (trackALoop != null)
            trackALoop.clip = loopClip;
    }

    public void SetTrackB(AudioClip startClip, AudioClip loopClip)
    {
        StopMusic();

        if (trackBStart != null)
            trackBStart.clip = startClip;

        if (trackBLoop != null)
            trackBLoop.clip = loopClip;
    }

    public void ClearTrackA()
    {
        StopMusic();

        if (trackAStart != null)
            trackAStart.clip = null;

        if (trackALoop != null)
            trackALoop.clip = null;
    }

    public void ClearTrackB()
    {
        StopMusic();

        if (trackBStart != null)
            trackBStart.clip = null;

        if (trackBLoop != null)
            trackBLoop.clip = null;
    }

    // ---------------------------------------
    // MUSIC CROSSFADE
    // ---------------------------------------

    public void CrossfadeToTrackA(float duration)
    {
        CrossfadeTo(0f, duration);
    }

    public void CrossfadeToTrackB(float duration)
    {
        CrossfadeTo(1f, duration);
    }

    public void SwapTracks()
    {
        if (musicBlend < 0.5f)
            CrossfadeToTrackB(defaultCrossfadeDuration);
        else
            CrossfadeToTrackA(defaultCrossfadeDuration);
    }

    private void CrossfadeTo(float target, float duration)
    {
        if (crossfadeRoutine != null)
            StopCoroutine(crossfadeRoutine);

        crossfadeRoutine = StartCoroutine(
            CrossfadeRoutine(target, duration)
        );
    }

    private IEnumerator CrossfadeRoutine(float target, float duration)
    {
        float start = musicBlend;
        float elapsed = 0f;

        target = Mathf.Clamp01(target);

        if (duration > 0f)
        {
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;

                float t = Mathf.Clamp01(elapsed / duration);

                // Smooth ease in / ease out
                t = t * t * (3f - 2f * t);

                musicBlend = Mathf.Lerp(start, target, t);

                ApplyVolumes();

                yield return null;
            }
        }

        musicBlend = target;
        ApplyVolumes();

        crossfadeRoutine = null;
    }

    // ---------------------------------------
    // AMBIANCE CLIP MANAGEMENT
    // ---------------------------------------

    private void ChangeAmbiance(AudioSource source, AudioClip clip)
    {
        if (source == null)
            return;

        StopAmbianceFade(source);

        source.Stop();
        source.clip = clip;

        ambianceFades[source] = 1f;

        ApplyVolumes();

        if (clip != null)
            source.Play();
    }

    public void SetAmbiance1(AudioClip clip)
    {
        ChangeAmbiance(ambiance1, clip);
    }

    public void SetAmbiance2(AudioClip clip)
    {
        ChangeAmbiance(ambiance2, clip);
    }

    public void ClearAmbiance1()
    {
        ChangeAmbiance(ambiance1, null);
    }

    public void ClearAmbiance2()
    {
        ChangeAmbiance(ambiance2, null);
    }

    // ---------------------------------------
    // AMBIANCE FADES
    // ---------------------------------------

    private void StopAmbianceFade(AudioSource source)
    {
        if (source == null)
            return;

        Coroutine routine;

        if (ambianceRoutines.TryGetValue(source, out routine))
        {
            if (routine != null)
                StopCoroutine(routine);

            ambianceRoutines.Remove(source);
        }
    }

    private void FadeAmbiance(AudioSource source, float target, float duration)
    {
        if (source == null)
            return;

        StopAmbianceFade(source);

        if (!ambianceFades.ContainsKey(source))
            ambianceFades[source] = 1f;

        ApplyVolumes();

        if (target > 0f && source.clip != null && !source.isPlaying)
            source.Play();

        ambianceRoutines[source] = StartCoroutine(
            FadeAmbianceRoutine(source, target, duration)
        );
    }

    private IEnumerator FadeAmbianceRoutine(
        AudioSource source, float target, float duration)
    {
        float start = ambianceFades[source];
        float elapsed = 0f;

        target = Mathf.Clamp01(target);

        if (duration > 0f)
        {
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;

                float t = Mathf.Clamp01(elapsed / duration);
                t = t * t * (3f - 2f * t);

                ambianceFades[source] =
                    Mathf.Lerp(start, target, t);

                ApplyVolumes();

                yield return null;
            }
        }

        ambianceFades[source] = target;
        ApplyVolumes();

        if (target <= 0f && source != null)
            source.Stop();

        ambianceRoutines.Remove(source);
    }

    public void FadeInAmbiance1(float duration)
    {
        FadeAmbiance(ambiance1, 1f, duration);
    }

    public void FadeOutAmbiance1(float duration)
    {
        FadeAmbiance(ambiance1, 0f, duration);
    }

    public void FadeInAmbiance2(float duration)
    {
        FadeAmbiance(ambiance2, 1f, duration);
    }

    public void FadeOutAmbiance2(float duration)
    {
        FadeAmbiance(ambiance2, 0f, duration);
    }

    // ---------------------------------------
    // AMBIANCE CLIP CHANGE WITH FADE
    // ---------------------------------------

    private void TransitionAmbiance(AudioSource source, AudioClip newClip, float fadeOutDuration, float fadeInDuration)
    {
        if (source == null)
            return;

        StartCoroutine(TransitionAmbianceRoutine(source, newClip, fadeOutDuration, fadeInDuration));
    }

    private IEnumerator TransitionAmbianceRoutine(AudioSource source, AudioClip newClip, float fadeOutDuration, float fadeInDuration)
    {
        // Fade out the current ambiance
        FadeAmbiance(source, 0f, fadeOutDuration);

        yield return new WaitForSecondsRealtime(Mathf.Max(0f, fadeOutDuration));

        // Replace the clip and reset its fade multiplier to zero
        ChangeAmbiance(source, null);

        source.clip = newClip;
        ambianceFades[source] = 0f;

        ApplyVolumes();

        // Fade in the new ambiance
        if (newClip != null)
        {
            FadeAmbiance(source, 1f, fadeInDuration);
        }
    }

    public void TransitionAmbiance1(AudioClip newClip, float fadeOutDuration, float fadeInDuration)
    {
        if (ambiance1 == null || ambiance1.clip == newClip)
            return;

        TransitionAmbiance(ambiance1, newClip, fadeOutDuration, fadeInDuration);
    }

    public void TransitionAmbiance2(AudioClip newClip, float fadeOutDuration, float fadeInDuration)
    {
        if (ambiance2 == null || ambiance2.clip == newClip)
            return;

        TransitionAmbiance(ambiance2, newClip, fadeOutDuration, fadeInDuration);
    }


    private void ChangeAmbianceWithFade(
        AudioSource source, AudioClip clip, float duration)
    {
        if (source == null)
            return;

        ChangeAmbiance(source, null);

        source.clip = clip;

        if (clip == null)
            return;

        ambianceFades[source] = 0f;
        ApplyVolumes();

        FadeAmbiance(source, 1f, duration);
    }

    public void SetAmbiance1WithFade(AudioClip clip, float duration)
    {
        ChangeAmbianceWithFade(ambiance1, clip, duration);
    }

    public void SetAmbiance2WithFade(AudioClip clip, float duration)
    {
        ChangeAmbianceWithFade(ambiance2, clip, duration);
    }

    public void CrossfadeAmbiance(bool useAmbiance2, float duration)
    {
        if (useAmbiance2)
        {
            FadeOutAmbiance1(duration);
            FadeInAmbiance2(duration);
        }
        else
        {
            FadeOutAmbiance2(duration);
            FadeInAmbiance1(duration);
        }
    }

    // ---------------------------------------
    // MUSIC AND SFX GROUP FADES
    // ---------------------------------------

    public void FadeInMusic(float duration)
    {
        FadeGroup(true, 1f, duration);
    }

    public void FadeOutMusic(float duration)
    {
        FadeGroup(true, 0f, duration);
    }

    public void FadeInSfx(float duration)
    {
        FadeGroup(false, 1f, duration);
    }

    public void FadeOutSfx(float duration)
    {
        FadeGroup(false, 0f, duration);
    }

    private void FadeGroup(bool music, float target, float duration)
    {
        if (music)
        {
            if (musicFadeRoutine != null)
                StopCoroutine(musicFadeRoutine);

            musicFadeRoutine = StartCoroutine(
                FadeGroupRoutine(true, target, duration)
            );
        }
        else
        {
            if (sfxFadeRoutine != null)
                StopCoroutine(sfxFadeRoutine);

            sfxFadeRoutine = StartCoroutine(
                FadeGroupRoutine(false, target, duration)
            );
        }
    }

    private IEnumerator FadeGroupRoutine(
        bool music, float target, float duration)
    {
        float start = music ? musicFade : sfxFade;
        float elapsed = 0f;

        target = Mathf.Clamp01(target);

        if (duration > 0f)
        {
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;

                float t = Mathf.Clamp01(elapsed / duration);
                t = t * t * (3f - 2f * t);

                float value = Mathf.Lerp(start, target, t);

                if (music)
                    musicFade = value;
                else
                    sfxFade = value;

                ApplyVolumes();

                yield return null;
            }
        }

        if (music)
        {
            musicFade = target;
            musicFadeRoutine = null;
        }
        else
        {
            sfxFade = target;
            sfxFadeRoutine = null;
        }

        ApplyVolumes();
    }

    // ---------------------------------------
    // QUEUE AUDIO FOR NEXT SCENE
    // ---------------------------------------

    public void QueueNextSceneAudio(
        string sceneName,
        bool replaceMusic,
        AudioClip aStart,
        AudioClip aLoop,
        AudioClip bStart,
        AudioClip bLoop,
        bool replaceAmbiance,
        AudioClip newAmbiance1,
        AudioClip newAmbiance2,
        float fadeInDuration)
    {
        pendingSceneAudio = true;
        pendingSceneName = sceneName;

        pendingReplaceMusic = replaceMusic;
        pendingReplaceAmbiance = replaceAmbiance;

        pendingTrackAStart = aStart;
        pendingTrackALoop = aLoop;
        pendingTrackBStart = bStart;
        pendingTrackBLoop = bLoop;

        pendingAmbiance1 = newAmbiance1;
        pendingAmbiance2 = newAmbiance2;

        pendingAudioFadeInDuration = Mathf.Max(
            0f, fadeInDuration
        );
    }

    // ---------------------------------------
    // COMPLETE SCENE AUDIO TRANSITION
    // ---------------------------------------

    private IEnumerator CompleteSceneAudioTransition()
    {
        // Allow new scene objects to initialize
        yield return null;

        if (!pendingSceneAudio)
            yield break;

        pendingSceneAudio = false;

        float fadeDuration = pendingAudioFadeInDuration;

        // ---------------------------------------
        // REPLACE MUSIC IF REQUESTED
        // ---------------------------------------

        if (pendingReplaceMusic)
        {
            // Ensure old music is completely silent
            FadeOutMusic(0f);

            StopMusic();

            // Replace Track A clips
            SetTrackA(
                pendingTrackAStart,
                pendingTrackALoop
            );

            // Replace Track B clips
            SetTrackB(
                pendingTrackBStart,
                pendingTrackBLoop
            );

            // Start the new song with Track A selected
            CrossfadeToTrackA(0f);

            // Schedule both synchronized versions
            PlayMusic();

            // Fade new music in
            FadeInMusic(fadeDuration);
        }

        // ---------------------------------------
        // REPLACE AMBIANCE IF REQUESTED
        // ---------------------------------------

        if (pendingReplaceAmbiance)
        {
            SetAmbiance1(pendingAmbiance1);
            SetAmbiance2(pendingAmbiance2);
        }

        // ---------------------------------------
        // RESTORE SFX IN NEW SCENE
        // ---------------------------------------

        // Reconnect objects that have spawned
        if (footsteps == null)
            FindFootsteps();

        // Always fade SFX back in
        FadeInSfx(fadeDuration);

        ApplyVolumes();
    }

    // ---------------------------------------
    // CLEANUP
    // ---------------------------------------

    void OnDestroy()
    {
        if (Instance != this)
            return;

        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (music_slider != null)
            music_slider.onValueChanged.RemoveListener(SetMusicVolume);

        if (sfx_slider != null)
            sfx_slider.onValueChanged.RemoveListener(SetSfxVolume);

        Instance = null;
    }
}
