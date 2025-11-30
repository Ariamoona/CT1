using UnityEngine;
using System.Collections;
using VContainer;
using VContainer.Unity;

public interface IPipeSpawner
{
    void StartSpawning();
    void StopSpawning();
}

public class PipeSpawner : IPipeSpawner, IStartable
{
    [Header("Spawn Settings")]
    public float spawnRate = 2f;
    public float minHeight = -1f;
    public float maxHeight = 2f;
    public float pipeSpeed = 2f;

    private readonly PipeFactory _pipeFactory;
    private readonly IGameStateManager _gameStateManager;

    private bool _isSpawning = true;
    private Coroutine _spawnCoroutine;
    private MonoBehaviour _coroutineRunner;

    [Inject]
    public PipeSpawner(
        PipeFactory pipeFactory,
        IGameStateManager gameStateManager)
    {
        _pipeFactory = pipeFactory;
        _gameStateManager = gameStateManager;

        var go = new GameObject("PipeSpawnerRunner");
        _coroutineRunner = go.AddComponent<MonoBehaviour>();
        Object.DontDestroyOnLoad(go);
    }

    public void Start()
    {
        StartSpawning();
    }

    public void StartSpawning()
    {
        _isSpawning = true;
        _spawnCoroutine = _coroutineRunner.StartCoroutine(SpawnPipes());
    }

    public void StopSpawning()
    {
        _isSpawning = false;
        if (_spawnCoroutine != null)
            _coroutineRunner.StopCoroutine(_spawnCoroutine);
    }

    IEnumerator SpawnPipes()
    {
        while (_isSpawning && _gameStateManager.CurrentState == GameState.Playing)
        {
            SpawnPipe();
            yield return new WaitForSeconds(spawnRate);
        }
    }

    void SpawnPipe()
    {
        float height = Random.Range(minHeight, maxHeight);
        Vector3 spawnPosition = new Vector3(8f, height, 0);

        var pipe = _pipeFactory.Create(spawnPosition);
        pipe.SetSpeed(pipeSpeed);
    }
}

public class PipeFactory
{
    private readonly GameObject _pipePrefab;

    public PipeFactory(GameObject pipePrefab)
    {
        _pipePrefab = pipePrefab;
    }

    public PipeController Create(Vector3 position)
    {
        var pipeObject = Object.Instantiate(_pipePrefab, position, Quaternion.identity);
        return pipeObject.GetComponent<PipeController>();
    }
}