using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public int maxHealth = 3;
    public float moveSpeed = 2f;

    private int currentHealth;
    private Rigidbody2D rb;
    private bool movingRight = true;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveDirection = movingRight ? 1f : -1f;
        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;


        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        gameObject.SetActive(false); 
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Wall"))
        {
            movingRight = !movingRight;
        }
    }
}