// SoundManager.cs
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip walkSound;
    public AudioClip jumpSound;
    public AudioClip shootSound;
    public AudioClip enemyHurtSound;
    public AudioClip enemyDeathSound;
    public AudioClip backgroundMusic;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayBackgroundMusic();
    }

    public void PlayWalkSound()
    {
        sfxSource.PlayOneShot(walkSound);
    }

    public void PlayJumpSound()
    {
        sfxSource.PlayOneShot(jumpSound);
    }

    public void PlayShootSound()
    {
        sfxSource.PlayOneShot(shootSound);
    }

    public void PlayEnemyHurtSound()
    {
        sfxSource.PlayOneShot(enemyHurtSound);
    }

    public void PlayEnemyDeathSound()
    {
        sfxSource.PlayOneShot(enemyDeathSound);
    }

    public void PlayBackgroundMusic()
    {
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.Play();
    }
}