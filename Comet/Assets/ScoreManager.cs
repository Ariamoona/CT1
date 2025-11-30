using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI References")]
    public Text scoreText;
    public Text highScoreText;

    private int highScore = 0;

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
        // Загружаем рекорд
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateHighScoreDisplay();
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = score.ToString();

        // Проверяем рекорд
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            UpdateHighScoreDisplay();
        }
    }

    void UpdateHighScoreDisplay()
    {
        if (highScoreText != null)
            highScoreText.text = "Best: " + highScore.ToString();
    }

    public void ResetGame()
    {
        UpdateScore(0);
    }
}