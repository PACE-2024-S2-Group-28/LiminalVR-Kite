using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ASpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] asteroidPrefabs; 
    [SerializeField] private float spawnRate = 2.0f;
    [SerializeField] private float goldenChance = 0.05f; 
    // 5% change to spawn golden Asteroid

    private void Start()
    {
        StartCoroutine(SpawnAsteroids());
    }

    IEnumerator SpawnAsteroids()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnRate);
            SpawnAsteroid();
        }
    }

    void SpawnAsteroid()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-100f, 100f), Random.Range(-100f, 100f), 0);


        GameObject asteroidPrefab = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)];
        //pick one asteroidPrefab
        GameObject asteroid = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);

    
        Renderer renderer = asteroid.GetComponent<Renderer>();
        if (renderer != null)
        {
            if (Random.value < goldenChance)
            {
                renderer.material.color = Color.yellow; 
                // gold Asteroid
            }
            else
            {
                renderer.material.color = Color.red; 
                // Red Asteroid
            }
        }
    }
}
