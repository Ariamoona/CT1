using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public float moveSpeed = 5f;
    public SpriteRenderer playerSpriteRenderer; 

    private int playerId;
    private bool isLocalPlayer;
    private NetworkManager networkManager;
    private Rigidbody2D rb; 
    private Vector2 lastPosition; 
    private Color currentColor;
    private float positionSendRate = 0.1f;
    private float lastPositionSendTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); 
        SetRandomColor();
        lastPosition = transform.position;
    }

    public void Initialize(int id, bool isLocal, NetworkManager netManager)
    {
        playerId = id;
        isLocalPlayer = isLocal;
        networkManager = netManager;

        gameObject.name = $"Player_{id}{(isLocal ? "_Local" : "")}";

        if (isLocal)
        {
            SetupCamera();
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        HandleInput();
        HandleNetworkUpdates();
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        HandleMovement();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetRandomColor();
            networkManager.SendColorChange(playerId, currentColor);
        }
    }

    private void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(moveHorizontal, moveVertical);
        rb.velocity = movement * moveSpeed;
    }

    private void HandleNetworkUpdates()
    {
        if (Time.time - lastPositionSendTime > positionSendRate)
        {
            if (Vector2.Distance(transform.position, lastPosition) > 0.01f)
            {
                networkManager.SendPosition(transform.position);
                lastPosition = transform.position;
            }
            lastPositionSendTime = Time.time;
        }
    }

    public void SetColor(Color color)
    {
        currentColor = color;
        if (playerSpriteRenderer != null)
            playerSpriteRenderer.color = color;
    }

    public void SetRandomColor()
    {
        currentColor = new Color(Random.value, Random.value, Random.value);
        if (playerSpriteRenderer != null)
            playerSpriteRenderer.color = currentColor;
    }

    private void SetupCamera()
    {
        GameObject cameraObj = Camera.main.gameObject;

        CameraFollow cameraFollow = cameraObj.GetComponent<CameraFollow>();
        if (cameraFollow == null)
        {
            cameraFollow = cameraObj.AddComponent<CameraFollow>();
        }

        cameraFollow.target = transform;

        Debug.Log("Camera setup completed for local player");
    }

    void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController otherPlayer = collision.gameObject.GetComponent<PlayerController>();
            if (otherPlayer != null)
            {
                SetRandomColor();
                if (networkManager != null)
                {
                    networkManager.SendColorChange(playerId, currentColor);
                }
            }
        }
    }
}