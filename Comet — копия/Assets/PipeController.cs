using UnityEngine;
using VContainer;

public class PipeController : MonoBehaviour
{
    private float _moveSpeed = 2f;
    private const float DestroyXPosition = -10f;

    private IGameStateManager _gameStateManager;

    [Inject]
    public void Construct(IGameStateManager gameStateManager)
    {
        _gameStateManager = gameStateManager;
    }

    public void SetSpeed(float speed)
    {
        _moveSpeed = speed;
    }

    void Update()
    {
        if (_gameStateManager.CurrentState != GameState.Playing) return;

        transform.position += Vector3.left * _moveSpeed * Time.deltaTime;

        if (transform.position.x < DestroyXPosition)
        {
            Destroy(gameObject);
        }
    }
}