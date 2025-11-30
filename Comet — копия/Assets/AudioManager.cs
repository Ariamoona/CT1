using UnityEngine;
using VContainer;

public interface IAudioManager
{
    void PlayFlap();
    void PlayHit();
    void PlayScore();
}

public class AudioManager : IAudioManager
{
    private readonly AudioSource _audioSource;
    private readonly AudioClip _flapSound;
    private readonly AudioClip _hitSound;
    private readonly AudioClip _scoreSound;

    [Inject]
    public AudioManager(AudioClip flapSound, AudioClip hitSound, AudioClip scoreSound)
    {
        _flapSound = flapSound;
        _hitSound = hitSound;
        _scoreSound = scoreSound;

        var gameObject = new GameObject("AudioManager");
        _audioSource = gameObject.AddComponent<AudioSource>();
        Object.DontDestroyOnLoad(gameObject);
    }

    public void PlayFlap()
    {
        _audioSource.PlayOneShot(_flapSound);
    }

    public void PlayHit()
    {
        _audioSource.PlayOneShot(_hitSound);
    }

    public void PlayScore()
    {
        _audioSource.PlayOneShot(_scoreSound);
    }
}