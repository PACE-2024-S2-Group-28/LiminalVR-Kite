using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public GameObject redAsteroidPrefab;
    public GameObject goldenAsteroidPrefab;
    public float spawnRate = 2.0f;
    public float goldenChance = 0.05f;  
    // 5% chance to spawn a golden asteroid

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
        Vector3 spawnPosition = new Vector3(Random.Range(-1000f, 1000f), Random.Range(-1000f, 1000f), 0);
        GameObject asteroidPrefab = Random.value < goldenChance ? goldenAsteroidPrefab : redAsteroidPrefab;
        Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);
    }
}
