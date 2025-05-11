using System;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Sources"), SerializeField]
    private AudioSource musicSource;

    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips"), SerializeField]
    private AudioClip backgroundMusic;

    [SerializeField] private AudioClip shootSfx;
    [SerializeField] private AudioClip hitSfx;
    [SerializeField] private AudioClip goalSfx;

    [Header("Audio Settings"), Range(0f, 1f)]
    public float musicVolume = 1f;

    [Range(0f, 1f)] public float sfxVolume = 1f;

    public enum SfxType
    {
        Shoot,
        Hit,
        Goal
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Set initial volumes
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;

        // Play background music
        PlayMusic(backgroundMusic);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (!clip) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void PlaySfx(SfxType clip)
    {
        AudioClip sfxClip = clip switch
        {
            SfxType.Shoot => shootSfx,
            SfxType.Hit => hitSfx,
            SfxType.Goal => goalSfx,
            _ => throw new ArgumentOutOfRangeException(nameof(clip), clip, null)
        };

        sfxSource.PlayOneShot(sfxClip, sfxVolume);
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        musicSource.volume = musicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
    }
}