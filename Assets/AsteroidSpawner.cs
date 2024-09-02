using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class AsteroidSpawner : MonoBehaviour
{
    [SerializeField]
    private Vector3 spawnBox = Vector3.one;

    [SerializeField]
    private GameObject[] asteroidFab;

    [SerializeField]
    private int maxAsteroids = 7;

    [SerializeField]
    private Vector3 startingDefaultVel = Vector3.zero;

    [SerializeField]
    private float
        spawnChance = .2f,
        spawnMinTime = .3f,
        spawnMaxTime = 2f,
        spawnRotationStrength = 5f,
        spawnExtraVelocityStrength = 2f;

    [SerializeField]
    private int spawnTickRate = 30;

    private float timer;
    private int activeAsteroids = 0;
    private int goldenAsteroidsCount = 0;
    private float goldenTimer = 0;
    [SerializeField] private float goldenAsteroidSpawningTime = 15;
    [SerializeField] private Vector3 goldenAsteroidFixedPosition = new Vector3(0, 5, 0);
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
        goldenTimer += Time.deltaTime;
    }

    private void TrySpawn()
    {
        if (activeAsteroids >= maxAsteroids) {
            timer = 0;
            return;
        }

        if (timer >= spawnMaxTime || (goldenTimer > goldenAsteroidSpawningTime && timer > spawnMinTime)) {
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

        int prefabIndex = (goldenTimer > goldenAsteroidSpawningTime) ? 1 : 0;
        if (prefabIndex == 1){ 
            goldenAsteroidsCount++;
            goldenTimer = 0;
        }
        GameObject asteroid = asteroidFab[prefabIndex];
        
        activeAsteroids++;
        timer = 0f;

         Vector3 spawnVec;
        if (prefabIndex == 1) {
            spawnVec = goldenAsteroidFixedPosition;
        } else {
            //spawn pos random
        spawnVec = Random.insideUnitSphere;
        spawnVec = Vector3.Scale(spawnVec, spawnBox);
        }

        var asteroidT = GameObject.Instantiate(asteroidFab[prefabIndex]).transform;
        asteroidT.parent = this.transform;
        asteroidT.position = transform.position + spawnVec * .5f;

        Color asteroidColor = (prefabIndex == 0) ? Color.red : Color.yellow; 
        // Assuming index 0 is red, 1 is golden
        MeshRenderer renderer = asteroidT.GetComponentInChildren<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material.color = asteroidColor;
        }

        //random rotation
        spawnVec = Random.insideUnitSphere * 360f;
        asteroidT.rotation = Quaternion.Euler(spawnVec);

        //angular velocity random
        var rb = asteroidT.GetComponentInChildren<Rigidbody>();
        spawnVec = Random.insideUnitSphere;
        rb.angularVelocity = spawnVec * spawnRotationStrength;

        //random rb velocity
        rb.velocity = startingDefaultVel + Random.insideUnitSphere * spawnExtraVelocityStrength;
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
