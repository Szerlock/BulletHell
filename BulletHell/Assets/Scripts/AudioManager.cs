using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer")]
    public AudioMixer audioMixer;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Boss Music Clips")]
    public AudioClip[] bossPhaseTracks;

    [Header("SFX Clips")]
    [SerializeField] private List<AudioClip> sfxClips;

    [SerializeField] private float fadeDuration = 1f;
    private Coroutine musicFadeCoroutine;
    private bool isFadingOut = false;
    private bool isFadingIn = false;


    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if (UIManager.Instance.backgroundUp && musicSource.isPlaying && !isFadingOut)
        {
            if (musicFadeCoroutine != null) StopCoroutine(musicFadeCoroutine);
            musicFadeCoroutine = StartCoroutine(FadeOutAndPause());
        }
        else if (!UIManager.Instance.backgroundUp && !musicSource.isPlaying && !isFadingIn)
        {
            if (musicFadeCoroutine != null) StopCoroutine(musicFadeCoroutine);
            musicFadeCoroutine = StartCoroutine(UnPauseAndFadeIn());
        }
    }

    private IEnumerator FadeOutAndPause()
    {
        isFadingOut = true;
        float startVolume = musicSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        musicSource.volume = 0f;
        musicSource.Pause();
        isFadingOut = false;

    }

    private IEnumerator UnPauseAndFadeIn()
    {
        isFadingIn = true;
        musicSource.UnPause();
        float targetVolume = 1f;
        musicSource.volume = 0f;

        for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, targetVolume, t / fadeDuration);
            yield return null;
        }

        musicSource.volume = targetVolume;
        isFadingIn = false;

    }

    public void PreloadBossMusic(int bossNumber)
    {
        int index1 = (bossNumber - 1) * 2;
        int index2 = index1 + 1;

        if (index1 >= 0 && index2 < bossPhaseTracks.Length)
        {
            LoadClip(bossPhaseTracks[index1]);
            LoadClip(bossPhaseTracks[index2]);
        }
    }

    private void LoadClip(AudioClip clip)
    {
        if (clip == null) return;

        if (clip.loadState != AudioDataLoadState.Loaded)
        {
            clip.LoadAudioData();
        }
        StartCoroutine(WaitForClipToLoad(clip));
    }

    private IEnumerator WaitForClipToLoad(AudioClip clip)
    {
        float timeout = 2f;
        float timer = 0f;

        while (clip.loadState == AudioDataLoadState.Loading && timer < timeout)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
        }

        if (clip.loadState == AudioDataLoadState.Loaded)
            Debug.Log($"[AudioManager] Clip {clip.name} loaded successfully.");
        else
            Debug.LogWarning($"[AudioManager] Failed to preload {clip.name}. Load state: {clip.loadState}");
    }

    public void UnloadBossMusic(int bossNumber)
    {
        int index1 = (bossNumber - 1) * 2;
        int index2 = index1 + 1;

        if (index1 >= 0 && index2 < bossPhaseTracks.Length)
        {
            bossPhaseTracks[index1]?.UnloadAudioData();
            bossPhaseTracks[index2]?.UnloadAudioData();
        }
    }

    // === MUSIC ===
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
        Debug.Log($"Play {clip}");
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }
    public void PlaySFX(string clipName)
    {
        AudioClip clip = sfxClips.Find(c => c.name == clipName);
        if (clip != null)
        {
            PlaySFX(clip);
        }
        else
        {
            Debug.LogWarning($"SFX Clip '{clipName}' not found in the list.");
        }
    }

    public void PlaySFXWithPitch(string clipName, float pitchMin = 0.95f, float pitchMax = 1.05f)
    {
        AudioClip clip = sfxClips.Find(c => c != null && c.name == clipName);
        if (clip == null)
        {
            Debug.LogWarning($"Audio clip '{clipName}' not found.");
            return;
        }

        sfxSource.pitch = Random.Range(pitchMin, pitchMax);
        sfxSource.PlayOneShot(clip);
        sfxSource.pitch = 1f;
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f);
    }
    public void PlayBossMusic(int bossNumber, int phase)
    {
        int index = (bossNumber - 1) * 2 + (phase - 1);
        if (index >= 0 && index < bossPhaseTracks.Length)
        {
            PlayMusic(bossPhaseTracks[index]);
        }
        else
        {
            Debug.LogWarning($"Invalid boss music index for Boss {bossNumber}, Phase {phase}");
        }
    }
}
