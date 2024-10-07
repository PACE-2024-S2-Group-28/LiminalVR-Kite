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

    [SerializeField, MinMaxSlider(0.1f, 10f)]
    private Vector2 asteroidSizeRange = new Vector2(1f, 5f);
    

    [SerializeField, MinMaxSlider(0f, 10f)]
    private Vector2 asteroidSpeedRange = new Vector2(1f, 5f);
    public Vector2 AsteroidSpeedRange { get => asteroidSpeedRange; set => asteroidSpeedRange = value; }

    [SerializeField, MinMaxSlider(0f, 50f)]
    private Vector2 spawnDistanceRange = new Vector2(10f, 20f);

    [SerializeField]
    private float
        spawnChance = .2f,
        spawnMinTime = .3f,
        spawnMaxTime = 2f,
        spawnRotationStrength = 5f,
        spawnExtraVelocityStrength = 2f;

    [SerializeField]
    private float spawnTickRate = 30;

    private float timer;
    private int activeAsteroids = 0;
    private int goldenAsteroidsCount = 0;
    private float goldenTimer = 0;
    [SerializeField] private float goldenAsteroidSpawningTime = 15;
    [SerializeField] private Vector3 goldenAsteroidFixedPosition = new Vector3(0, 5, 0);
    private float gameTimer = 0;

    [SerializeField] public float maxSpeed = 10f;
    [SerializeField] public float minSpeed = 1f;
    [SerializeField] public float acceleration = 1f;
    [SerializeField] public float slowAmount = 2f;
    

    private void Start()
    {
        InvokeRepeating(nameof(TrySpawn), 0f, 1f / (float)spawnTickRate);
        //UpdateSpawn();
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

    public void AdjustSpawnTickRate(float rate)
    {
        rate = Mathf.Max(Mathf.Epsilon, rate);

        //spawnTickRate = Mathf.Clamp(rate, 1, 30); 
        spawnMinTime = 1f / (rate*1.2f);
        spawnMaxTime = 1f / (rate*.8f);
        //UpdateSpawn();
    }

    private void UpdateSpawn()
    {
        CancelInvoke(nameof(TrySpawn));
        InvokeRepeating(nameof(TrySpawn), 0f, 1f / spawnTickRate);
    }
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
        Vector3 spawnVec = Vector3.zero; 
        int prefabIndex = (goldenTimer > goldenAsteroidSpawningTime) ? 1 : 0;
        if (prefabIndex == 1){ 
            goldenAsteroidsCount++;
            goldenTimer = 0;
            AsteroidGameManager.Instance.RecordGoldenAsteroidSpawn(spawnVec);
        }
        GameObject asteroid = asteroidFab[prefabIndex];
        
        activeAsteroids++;
        timer = 0f;

        //Vector3 spawnVec;
        if (prefabIndex == 1) {
            spawnVec = goldenAsteroidFixedPosition;
        } else {
            spawnVec = Random.insideUnitSphere;
            spawnVec = Vector3.Scale(spawnVec, spawnBox);
        }

        var asteroidT = GameObject.Instantiate(asteroidFab[prefabIndex]).transform;
        asteroidT.parent = this.transform;
        
        
        float spawnDistance = Random.Range(spawnDistanceRange.x, spawnDistanceRange.y);
        asteroidT.position = transform.position + spawnVec;
        //Calculate spawn distance from player

        
        float size = Random.Range(asteroidSizeRange.x, asteroidSizeRange.y);
        asteroidT.localScale = Vector3.one * size;
        //Randomize asteroid size
        Color asteroidColor = (prefabIndex == 0) ? Color.red : Color.yellow;
        MeshRenderer renderer = asteroidT.GetComponentInChildren<MeshRenderer>();

        //random rotation
        spawnVec = Random.insideUnitSphere * 360f;
        asteroidT.rotation = Quaternion.Euler(spawnVec);

        //angular velocity random
        var rb = asteroidT.GetComponentInChildren<Rigidbody>();
        spawnVec = Random.insideUnitSphere;
        rb.angularVelocity = spawnVec * spawnRotationStrength;

        //set starting velocity + random component based on speed range, lerped towards a direction to player
        float speed = Random.Range(asteroidSpeedRange.x, asteroidSpeedRange.y);
        Vector3 velToPlayer = -asteroidT.position.normalized * startingDefaultVel.magnitude;
        Vector3 lerpedVel = Vector3.Lerp(startingDefaultVel, velToPlayer, .8f);
        rb.velocity = lerpedVel + Random.insideUnitSphere.normalized * speed;
    }

    public void DestroyAsteroid(bool isGoldAsteroid)
    {
        AsteroidGameManager.Instance.HandleAsteroidDestruction(isGoldAsteroid);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, spawnBox);
    }
}
