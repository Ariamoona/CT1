using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Parallax Settings")]
    public float parallaxSpeed = 0.5f;
    public bool loop = true;

    private float spriteWidth;
    private Vector2 startPosition;

    void Start()
    {
        startPosition = transform.position;
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        float newPosition = Mathf.Repeat(Time.time * parallaxSpeed, spriteWidth);
        transform.position = startPosition + Vector2.left * newPosition;
    }
}