using System.Collections;
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
    public AudioClip bulletSFX;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
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
