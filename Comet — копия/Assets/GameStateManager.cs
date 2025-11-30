using UnityEngine;
using VContainer;
using VContainer.Unity;

public enum GameState
{
    Menu,
    Playing,
    GameOver
}

public interface IGameStateManager
{
    GameState CurrentState { get; }
    void StartGame();
    void GameOver();
    void RestartGame();
}

public class GameStateManager : IGameStateManager, IStartable
{
    public GameState CurrentState { get; private set; }

    private readonly IScoreManager _scoreManager;
    private readonly IPipeSpawner _pipeSpawner;

    [Inject]
    public GameStateManager(IScoreManager scoreManager, IPipeSpawner pipeSpawner)
    {
        _scoreManager = scoreManager;
        _pipeSpawner = pipeSpawner;
    }

    public void Start()
    {
        CurrentState = GameState.Menu;
        StartGame(); 
    }

    public void StartGame()
    {
        CurrentState = GameState.Playing;
        _scoreManager.ResetScore();
        _pipeSpawner.StartSpawning();
    }

    public void GameOver()
    {
        CurrentState = GameState.GameOver;
        _pipeSpawner.StopSpawning();
    }

    public void RestartGame()
    {
        var pipes = GameObject.FindGameObjectsWithTag("Pipe");
        foreach (var pipe in pipes)
        {
            Object.Destroy(pipe);
        }

        StartGame();
    }
}