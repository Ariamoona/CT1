using UnityEngine;
using VContainer;
using UnityEngine.SceneManagement;

public class CometController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float flapForce = 5f;
    public float rotationSpeed = 2f;
    public float maxRotation = 30f;
    public float fallRotation = -90f;

    private Rigidbody2D _rb;
    private bool _isAlive = true;
    private int _currentScore = 0;

    private IGameStateManager _gameStateManager;
    private IScoreManager _scoreManager;
    private IAudioManager _audioManager;
    private IPipeSpawner _pipeSpawner;
    private GameObject _gameOverPanel;

    [Inject]
    public void Construct(
        IGameStateManager gameStateManager,
        IScoreManager scoreManager,
        IAudioManager audioManager,
        IPipeSpawner pipeSpawner,
        GameObject gameOverPanel)
    {
        _gameStateManager = gameStateManager;
        _scoreManager = scoreManager;
        _audioManager = audioManager;
        _pipeSpawner = pipeSpawner;
        _gameOverPanel = gameOverPanel;
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        _rb.velocity = Vector2.up * flapForce;
    }

    void Update()
    {
        if (_gameStateManager.CurrentState != GameState.Playing) return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            Flap();
        }

        UpdateRotation();

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void Flap()
    {
        _rb.velocity = new Vector2(0, flapForce);
        _audioManager.PlayFlap();
    }

    void UpdateRotation()
    {
        float targetRotation;

        if (_rb.velocity.y > 0)
        {
            targetRotation = Mathf.Lerp(0, maxRotation, _rb.velocity.y / flapForce);
        }
        else
        {
            targetRotation = Mathf.Lerp(0, fallRotation, -_rb.velocity.y / flapForce);
        }

        transform.rotation = Quaternion.Euler(0, 0, targetRotation);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (_gameStateManager.CurrentState != GameState.Playing) return;

        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Pipe"))
        {
            GameOver();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_gameStateManager.CurrentState != GameState.Playing) return;

        if (other.CompareTag("ScoreZone"))
        {
            AddScore();
            _audioManager.PlayScore();
            Destroy(other.gameObject);
        }
    }

    void AddScore()
    {
        _currentScore++;
        _scoreManager.UpdateScore(_currentScore);
    }

    void GameOver()
    {
        _isAlive = false;
        _audioManager.PlayHit();
        _gameStateManager.GameOver();

        if (_gameOverPanel != null)
        {
            _gameOverPanel.SetActive(true);
            var panel = _gameOverPanel.GetComponent<GameOverPanel>();
            if (panel != null)
            {
                panel.SetScore(_currentScore, _scoreManager.HighScore);
            }
        }

        _rb.simulated = false;
    }
}