using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverPanel : MonoBehaviour
{
    [Header("UI Elements")]
    public Text scoreText;
    public Text highScoreText;
    public GameObject newHighScoreText;

    public void SetScore(int score)
    {
        scoreText.text = "Score: " + score.ToString();

        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = "Best: " + highScore.ToString();

        // Показываем "New Record!" если побили рекорд
        if (score > highScore)
        {
            newHighScoreText.SetActive(true);
        }
    }

    public void OnRestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnMenuButton()
    {
        // Загрузка главного меню (если есть)
        SceneManager.LoadScene("MainMenu");
    }
}