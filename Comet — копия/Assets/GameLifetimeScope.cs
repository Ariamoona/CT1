using VContainer;
using VContainer.Unity;
using UnityEngine;

public class GameLifetimeScope : LifetimeScope
{
    [Header("Prefabs")]
    public GameObject cometPrefab;
    public GameObject pipePrefab;

    [Header("UI")]
    public GameObject gameOverPanel;
    public UnityEngine.UI.Text scoreText;
    public UnityEngine.UI.Text highScoreText;

    [Header("Audio")]
    public AudioClip flapSound;
    public AudioClip hitSound;
    public AudioClip scoreSound;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<ScoreManager>(Lifetime.Singleton).As<IScoreManager>();
        builder.Register<PipeSpawner>(Lifetime.Singleton).As<IPipeSpawner>();
        builder.Register<GameStateManager>(Lifetime.Singleton).As<IGameStateManager>();
        builder.Register<AudioManager>(Lifetime.Singleton).As<IAudioManager>();

        builder.RegisterInstance(pipePrefab);
        builder.Register<PipeFactory>(Lifetime.Singleton);


        builder.RegisterInstance(gameOverPanel);
        builder.RegisterInstance(scoreText);
        builder.RegisterInstance(highScoreText);

        builder.RegisterInstance(flapSound);
        builder.RegisterInstance(hitSound);
        builder.RegisterInstance(scoreSound);
    }
}