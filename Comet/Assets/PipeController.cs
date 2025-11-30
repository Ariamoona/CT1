using UnityEngine;

public class PipeController : MonoBehaviour
{
    [Header("Pipe Settings")]
    public float moveSpeed = 2f;
    public float destroyXPosition = -10f;

    void Update()
    {
        // ƒвижение трубы влево
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;

        // ”ничтожение трубы за пределами экрана
        if (transform.position.x < destroyXPosition)
        {
            Destroy(gameObject);
        }
    }
}