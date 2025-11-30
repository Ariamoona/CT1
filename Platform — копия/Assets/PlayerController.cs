using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 15f;
    public float bulletLifetime = 2f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool canJump = true;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleShooting();
        HandleAnimation();

        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
    }

    void HandleMovement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        if (moveInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveInput), 1, 1);
        }
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && canJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            canJump = false;
        }
    }

    void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

            float direction = Mathf.Sign(transform.localScale.x);
            bulletRb.velocity = new Vector2(direction * bulletSpeed, 0);

            Destroy(bullet, bulletLifetime);

        }
    }

    void HandleAnimation()
    {
        bool isWalking = Mathf.Abs(rb.velocity.x) > 0.1f;
        animator.SetBool("IsWalking", isWalking);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            canJump = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Finish"))
        {
            GameManager.Instance.ShowWinPanel();
        }
    }
}