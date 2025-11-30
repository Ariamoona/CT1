using UnityEngine;
using System.Collections;

public class PipeSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject pipePrefab;
    public float spawnRate = 2f;
    public float minHeight = -1f;
    public float maxHeight = 2f;
    public float pipeSpeed = 2f;

    private bool isSpawning = true;
    private Coroutine spawnCoroutine;

    void Start()
    {
        StartSpawning();
    }

    public void StartSpawning()
    {
        isSpawning = true;
        spawnCoroutine = StartCoroutine(SpawnPipes());
    }

    public void StopSpawning()
    {
        isSpawning = false;
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);
    }

    IEnumerator SpawnPipes()
    {
        while (isSpawning)
        {
            SpawnPipe();
            yield return new WaitForSeconds(spawnRate);
        }
    }

    void SpawnPipe()
    {
        // Случайная высота
        float height = Random.Range(minHeight, maxHeight);
        Vector3 spawnPosition = new Vector3(transform.position.x, height, 0);

        GameObject pipe = Instantiate(pipePrefab, spawnPosition, Quaternion.identity);

        // Настраиваем скорость движения трубы
        PipeController pipeController = pipe.GetComponent<PipeController>();
        if (pipeController != null)
        {
            pipeController.moveSpeed = pipeSpeed;
        }
    }
}