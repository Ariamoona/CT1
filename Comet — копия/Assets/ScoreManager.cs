using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

public interface IScoreManager
{
    void UpdateScore(int score);
    void ResetScore();
    int HighScore { get; }
    int CurrentScore { get; }
}

public class ScoreManager : IScoreManager, IStartable
{
    private readonly Text _scoreText;
    private readonly Text _highScoreText;

    public int HighScore { get; private set; }
    public int CurrentScore { get; private set; }

    [Inject]
    public ScoreManager(Text scoreText, Text highScoreText)
    {
        _scoreText = scoreText;
        _highScoreText = highScoreText;
    }

    public void Start()
    {
        HighScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateHighScoreDisplay();
        ResetScore();
    }

    public void UpdateScore(int score)
    {
        CurrentScore = score;

        if (_scoreText != null)
            _scoreText.text = CurrentScore.ToString();

        if (CurrentScore > HighScore)
        {
            HighScore = CurrentScore;
            PlayerPrefs.SetInt("HighScore", HighScore);
            UpdateHighScoreDisplay();
        }
    }

    public void ResetScore()
    {
        CurrentScore = 0;
        UpdateScore(0);
    }

    private void UpdateHighScoreDisplay()
    {
        if (_highScoreText != null)
            _highScoreText.text = "Best: " + HighScore.ToString();
    }
}