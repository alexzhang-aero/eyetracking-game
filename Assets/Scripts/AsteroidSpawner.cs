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
    public float initialDelay = 5.0f;     // Initial delay before asteroid spawns

    void Start()
    {
        // Start the coroutine to delay the spawning process
        StartCoroutine(StartAsteroidSpawning());
    }

    // Coroutine that waits for the initial delay and then starts spawning asteroids
    IEnumerator StartAsteroidSpawning()
    {
        // Wait for the initial delay time
        yield return new WaitForSeconds(initialDelay);

        // Start the asteroid spawning loop after the delay
        while (true)
        {
            timeSinceLastSpawn += Time.deltaTime;

            // Check if it's time to spawn a new asteroid
            if (timeSinceLastSpawn >= spawnInterval)
            {
                SpawnAsteroid();
                timeSinceLastSpawn = 0.0f; // Reset timer after spawning
            }

            yield return null; // Wait for the next frame
        }
    }

    void SpawnAsteroid()
    {
        // Select a random asteroid from the array
        int randomIndex = Random.Range(0, asteroidPrefabs.Length);
        GameObject asteroid = Instantiate(asteroidPrefabs[randomIndex]);

        // Set a random spawn position within the X range and above the screen (Y position)
        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
        asteroid.transform.position = new Vector3(randomX, 6.0f, 0.0f);  // Above screen

        // Add a downward force to make the asteroid fall
        Rigidbody2D rb = asteroid.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = new Vector2(0, -asteroidSpeed);  // Move asteroid downward
        }
    }
}

