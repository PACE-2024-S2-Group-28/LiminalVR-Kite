using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class AsteroidSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject
        asteroidFab,
        goldAsteroidFab;

    [SerializeField]
    private Vector3 spawnBox = Vector3.one;

    [SerializeField]
    private int maxAsteroids = 7;

    [SerializeField]
    private Vector3 startingDefaultVel = Vector3.zero;

    [SerializeField, MinMaxSlider(0f, 1f)]
    private Vector2 minMaxVelLerpToPlayer = new Vector2(.6f, .9f);

    [SerializeField, MinMaxSlider(0f, 5f)]
    private Vector2
        minMaxSpawnTime = new Vector2(.3f, 2f),
        minMaxRotationSpeed = new Vector2(1.2f, 2f),
        minMaxAsteroidVelMult = new Vector2(.8f, 1.2f),
        minMaxAsteroidSizeMult = new Vector2(1f, 4f);
    private float getWithinMinMax(Vector2 range) => Random.Range(range.x, range.y);
    public Vector2 MinMaxSpawnTime {
        get { return minMaxSpawnTime; }
        set { minMaxSpawnTime = value; }
    }

    private float timer;
    private int activeAsteroids = 0;
    private int goldenAsteroidsCount = 0;
    private float gameTimer = 0;

    [SerializeField] public float maxSpeed = 10f;
    [SerializeField] public float minSpeed = 1f;
    [SerializeField] public float acceleration = 1f;
    [SerializeField] public float slowAmount = 2f;


    private void Start()
    {
        StartCoroutine(AsteroidSpawnRoutine());
    }

    private IEnumerator AsteroidSpawnRoutine()
    {
        yield return new WaitForSeconds(minMaxSpawnTime.y);

        while (true) {
            TrySpawnAsteroid();
            yield return new WaitForSeconds(getWithinMinMax(minMaxSpawnTime));
        }
    }

    #region subbing
    private void OnEnable()
    {
        RockDestroyer.SEvent_RockDestroyed.AddListener(CountRockDestroyed);
    }

    private void OnDisable()
    {
        RockDestroyer.SEvent_RockDestroyed.RemoveListener(CountRockDestroyed);
    }

    private void CountRockDestroyed() { activeAsteroids--; }
    #endregion

    public void AdjustSpawnRate(float timeToSpawn, float variance = .2f)
    {
        timeToSpawn = Mathf.Max(Mathf.Epsilon, timeToSpawn);

        minMaxSpawnTime.x = 1f / (timeToSpawn * (1 + variance));
        minMaxSpawnTime.y = 1f / (timeToSpawn * (1 - variance));
    }

    void Update()
    {
        timer += Time.deltaTime;
    }

    private void TrySpawnAsteroid()
    {

        if (activeAsteroids >= maxAsteroids) {
            timer = 0;
            return;
        }

        SpawnAsteroid();
    }

    [Button]
    public void SpawnAsteroid(bool isGold = false, Vector3? manualPos = null)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif

        timer = 0f;
        //normal or gold asteroid
        GameObject asteroidToSpawnFab;
        if (isGold) {
            goldenAsteroidsCount++;
            asteroidToSpawnFab = goldAsteroidFab;
        }
        else {
            activeAsteroids++;
            asteroidToSpawnFab = asteroidFab;
        }

        //spawning
        var asteroidT = GameObject.Instantiate(asteroidToSpawnFab).transform;
        asteroidT.parent = this.transform;

        //position
        if (isGold) {
            asteroidT.position = manualPos.Value;
        }
        else {
            Vector3 spawnVec = Random.insideUnitSphere;
            spawnVec = Vector3.Scale(spawnVec, spawnBox);
            asteroidT.position = transform.position + spawnVec;
        }

        //Randomize asteroid size
        float sizeMult = getWithinMinMax(minMaxAsteroidSizeMult);
        asteroidT.localScale = Vector3.one * sizeMult;

        //random rotation
        Vector3 rotVec = Random.insideUnitSphere * 360f;
        asteroidT.rotation = Quaternion.Euler(rotVec);

        //angular velocity random
        var rb = asteroidT.GetComponentInChildren<Rigidbody>();
        rotVec = Random.insideUnitSphere;
        rb.angularVelocity = rotVec * getWithinMinMax(minMaxRotationSpeed);

        //set starting velocity + random component based on speed range, lerped towards a direction to player
        float speedMult = getWithinMinMax(minMaxAsteroidVelMult);
        Vector3 vel = startingDefaultVel * speedMult;
        Vector3 velToPlayer = -asteroidT.position.normalized * vel.magnitude;
        Vector3 lerpedVel = Vector3.Lerp(vel, velToPlayer, getWithinMinMax(minMaxVelLerpToPlayer));
        rb.velocity = lerpedVel;
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
