using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public GameObject[] asteroidPrefabs;  // Array to store the 3 asteroid prefabs
    public float spawnInterval = 2.0f;    // Time between each asteroid spawn
    public float spawnRangeX = 8.0f;      // Horizontal range for spawning asteroids
    public float asteroidSpeed = 5.0f;    // Speed at which asteroids fall
    private float timeSinceLastSpawn = 0.0f;
    public float initialDelay = 10.0f;     // Initial delay before asteroid spawns

    void Start()
    {
        // Start the coroutine to delay the spawning process
        StartCoroutine(StartAsteroidSpawning());
    }

    IEnumerator StartAsteroidSpawning()
    {
        yield return new WaitForSeconds(initialDelay);

        // Start the asteroid spawning loop after the delay
        while (true)
        {
            timeSinceLastSpawn += Time.deltaTime;

            if (timeSinceLastSpawn >= spawnInterval)
            {
                SpawnAsteroid();
                timeSinceLastSpawn = 0.0f; // Reset timer
            }

            yield return null; 
        }
    }

    void SpawnAsteroid()
    {
        // Select a random asteroid art
        int randomIndex = Random.Range(0, asteroidPrefabs.Length);
        GameObject asteroid = Instantiate(asteroidPrefabs[randomIndex]);

        // Set a random spawn position 
        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
        asteroid.transform.position = new Vector3(randomX, 6.0f, 0.0f); 

        // Add a downward force to make the asteroid fall
        Rigidbody2D rb = asteroid.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = new Vector2(0, -asteroidSpeed);
        }
    }
}

