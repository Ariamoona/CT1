using UnityEngine;
using UnityEngine.SceneManagement;

public class CometController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float flapForce = 5f;
    public float rotationSpeed = 2f;
    public float maxRotation = 30f;
    public float fallRotation = -90f;

    [Header("Game References")]
    public GameObject gameOverPanel;
    public PipeSpawner pipeSpawner;
    public AudioClip flapSound;
    public AudioClip hitSound;
    public AudioClip scoreSound;

    private Rigidbody2D rb;
    private bool isAlive = true;
    private AudioSource audioSource;
    private float startTime;
    private int score = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        startTime = Time.time;

        // Начальный импульс вверх
        rb.velocity = Vector2.up * flapForce;
    }

    void Update()
    {
        if (!isAlive) return;

        // Управление - клик или пробел
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            Flap();
        }

        // Поворот кометы в зависимости от скорости
        UpdateRotation();
    }

    void Flap()
    {
        rb.velocity = new Vector2(0, flapForce);
        audioSource.PlayOneShot(flapSound);
    }

    void UpdateRotation()
    {
        float targetRotation;

        if (rb.velocity.y > 0)
        {
            // Летим вверх - поворачиваем вверх
            targetRotation = Mathf.Lerp(0, maxRotation, rb.velocity.y / flapForce);
        }
        else
        {
            // Падаем - поворачиваем вниз
            targetRotation = Mathf.Lerp(0, fallRotation, -rb.velocity.y / flapForce);
        }

        transform.rotation = Quaternion.Euler(0, 0, targetRotation);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isAlive) return;

        // Столкновение с землей или трубами
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Pipe"))
        {
            GameOver();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isAlive) return;

        // Прохождение через трубу - получаем очко
        if (other.CompareTag("ScoreZone"))
        {
            AddScore();
            audioSource.PlayOneShot(scoreSound);
            Destroy(other.gameObject); // Уничтожаем зону счета
        }
    }

    void AddScore()
    {
        score++;
        // Обновляем UI счета
        ScoreManager.Instance.UpdateScore(score);
    }

    void GameOver()
    {
        isAlive = false;
        audioSource.PlayOneShot(hitSound);

        // Останавливаем спавн труб
        if (pipeSpawner != null)
            pipeSpawner.StopSpawning();

        // Показываем game over панель
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            gameOverPanel.GetComponent<GameOverPanel>().SetScore(score);
        }

        // Отключаем физику
        rb.simulated = false;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}