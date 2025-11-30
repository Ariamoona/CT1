using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class GameOverPanel : MonoBehaviour
{
    [Header("UI Elements")]
    public Text scoreText;
    public Text highScoreText;
    public GameObject newHighScoreText;

    private IScoreManager _scoreManager;
    private IGameStateManager _gameStateManager;

    [Inject]
    public void Construct(IScoreManager scoreManager, IGameStateManager gameStateManager)
    {
        _scoreManager = scoreManager;
        _gameStateManager = gameStateManager;
    }

    public void SetScore(int score, int highScore)
    {
        scoreText.text = "Score: " + score.ToString();
        highScoreText.text = "Best: " + highScore.ToString();

        if (score > highScore)
        {
            newHighScoreText.SetActive(true);
        }
        else
        {
            newHighScoreText.SetActive(false);
        }
    }

    public void OnRestartButton()
    {
        _gameStateManager.RestartGame();
        gameObject.SetActive(false);

        var comet = FindObjectOfType<CometController>();
        if (comet != null)
        {
            comet.transform.position = Vector3.zero;
            var rb = comet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.simulated = true;
            }
        }
    }

    public void OnMenuButton()
    {
        Debug.Log("Return to menu");
    }
}