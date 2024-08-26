using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class AsteroidSpawner : MonoBehaviour
{
    [SerializeField]
    private Vector3 spawnBox = Vector3.one;

    [SerializeField]
    private GameObject asteroidFab;

    [SerializeField]
    private int maxAsteroids = 7;

    [SerializeField]
    private float
        spawnChance = .2f,
        spawnMinTime = .3f,
        spawnMaxTime = 2f,
        spawnRotationStrength = 5f;

    [SerializeField]
    private int spawnTickRate = 30;

    private float timer;
    private int activeAsteroids = 0;

    private void Start()
    {
        InvokeRepeating(nameof(TrySpawn), 0f, 1f / (float)spawnTickRate);
    }

    private void OnEnable()
    {
        RockDestroyer.SEvent_RockDestroyed.AddListener(CountRockDestroyed);
    }

    private void OnDisable()
    {
        RockDestroyer.SEvent_RockDestroyed.RemoveListener(CountRockDestroyed);
    }

    private void CountRockDestroyed() { activeAsteroids--; }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
    }

    private void TrySpawn()
    {
        if (activeAsteroids >= maxAsteroids) {
            timer = 0;
            return;
        }

        if (timer >= spawnMaxTime) {
            SpawnAsteroid();
            return;
        }
        
        if (timer > spawnMinTime) {
            if(Random.Range(0f, 1f)<=spawnChance) {
                SpawnAsteroid();
            }
        }
    }

    [Button]
    private void SpawnAsteroid()
    {
        #if UNITY_EDITOR
        if (!Application.isPlaying) return;
        #endif

        activeAsteroids++;
        timer = 0f;

        //spawn pos random
        float x = Mathf.PerlinNoise(0, Time.time);
        float y = Mathf.PerlinNoise(1, Time.time);
        float z = Mathf.PerlinNoise(2, Time.time);
        var spawnVec = new Vector3(x, y, z) * 2f - Vector3.one;
        spawnVec = Vector3.Scale(spawnVec, spawnBox);

        var asteroidT = GameObject.Instantiate(asteroidFab).transform;
        asteroidT.parent = this.transform;
        asteroidT.position = transform.position + spawnVec;

        //spawn rot random
        x = Mathf.PerlinNoise(3, Time.time);
        y = Mathf.PerlinNoise(4, Time.time);
        z = Mathf.PerlinNoise(5, Time.time);
        spawnVec = new Vector3(x, y, z) * 360f;

        asteroidT.rotation = Quaternion.Euler(spawnVec);

        //angular velocity random
        var rb = asteroidT.GetComponentInChildren<Rigidbody>();
        x = Mathf.PerlinNoise(6, Time.time);
        y = Mathf.PerlinNoise(7, Time.time);
        z = Mathf.PerlinNoise(8, Time.time);
        spawnVec = new Vector3(x, y, z) * 2f - Vector3.one;

        rb.angularVelocity = spawnVec * spawnRotationStrength;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, Vector3.one * spawnBox.x);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, Vector3.one * spawnBox.y);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, Vector3.one * spawnBox.z);
    }
}
