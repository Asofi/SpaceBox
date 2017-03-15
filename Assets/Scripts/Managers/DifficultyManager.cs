using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour {

    public int Level = 0;
    [Space]
    [Header("Orbits")]
    public int WhenChangeOrbitsCount;
    private int orbitsCount = 3;
    public Vector2 OrbitMinSpeed;
    public Vector2 OrbitMaxSpeed;
    private Vector2 orbitCurSpeed;
    public float OrbitSpeedStep;
    [Space]
    [Header("Asteroids")]
    public float MinAsteroidSpeed;
    public float MaxAsteroidSpeed;
    private float asteroidSpeed;
    private float originMinSpeed;
    private float originMaxSpeed;
    public float AsteroidSpeedStep;

    public float MinAsteroidSpawnTime = 1;
    public float MaxAsteroidSpawnTime = 5;
    private float asteroidSpawnTime;
    public float AsteroidSpawnTimeDecrease = 0.25f;


    public Transform AsteroidPrefab;
    private float timeBetweenSpawnAsteroids;
    public float AsteroidSpawnRadius;

    [Space]
    [Header("Resize")]
    public Transform[] ObjectsToResize;
    private Vector3[] originSizes;
    private Vector3[] startCubeSizes;
    private float resizeFactor;

    // Use this for initialization
    void Start () {
        EventManager.OnGameStart += OnGameStart;
        EventManager.OnGameOver += OnGameOver;
        EventManager.OnLevelUp += OnLevelUp;
        originMinSpeed = MinAsteroidSpeed;
        originMaxSpeed = MaxAsteroidSpeed;
        asteroidSpawnTime = MaxAsteroidSpawnTime;
        originSizes = new Vector3[ObjectsToResize.Length];
        startCubeSizes = new Vector3[3];
        for (int i = 0; i < ObjectsToResize.Length; i++)
            originSizes[i] = ObjectsToResize[i].localScale;

        for (int i = 0; i < 3; i++)
            startCubeSizes[i] = originSizes[0] * ((5f+(5-(i+3)))/(i+3));

        ChangeSizes();
    }

    void OnGameStart()
    {
        Level = 1;
        asteroidSpawnTime = MaxAsteroidSpawnTime;
        MinAsteroidSpeed = originMinSpeed;
        MaxAsteroidSpeed = originMaxSpeed;
        orbitsCount = 3;

        orbitCurSpeed = OrbitMinSpeed;

        ChangeSizes();

        StartCoroutine(SpawnAsteroids());
        
    }

    void OnGameOver()
    {
        StopAllCoroutines();
    }

    void OnLevelUp()
    {
        Level++;
        if (Level % WhenChangeOrbitsCount == 0 && orbitsCount < 5)
            orbitsCount++;

        ChangeSizes();

        float newChance = asteroidSpawnTime - AsteroidSpawnTimeDecrease;
        asteroidSpawnTime = Mathf.Clamp(newChance, MinAsteroidSpawnTime, MaxAsteroidSpawnTime);

        MinAsteroidSpeed += AsteroidSpeedStep;
        MaxAsteroidSpeed += AsteroidSpeedStep;

        orbitCurSpeed.x = Mathf.Clamp(orbitCurSpeed.x + OrbitSpeedStep, OrbitMinSpeed.x, OrbitMaxSpeed.x);
        orbitCurSpeed.y = Mathf.Clamp(orbitCurSpeed.y + OrbitSpeedStep, OrbitMinSpeed.y, OrbitMaxSpeed.y);
    }

    public float GetAsteroidSpeed()
    {
        return Random.Range(MinAsteroidSpeed, MaxAsteroidSpeed);
    }

    public float GetOrbitSpeed()
    {
        return Random.Range(orbitCurSpeed.x, orbitCurSpeed.y);
    }

    public int GetOrbitsCount()
    {
        return orbitsCount;
    }

    void ChangeSizes()
    {
        float scaleFactor = (5 + (5-orbitsCount)) / (float)orbitsCount;
        for (int i = 0; i < ObjectsToResize.Length; i++)
            ObjectsToResize[i].localScale = originSizes[i] * scaleFactor;
    }

    public Vector3 GetCubeSize()
    {
        return startCubeSizes[orbitsCount - 3];
    }

    IEnumerator SpawnAsteroids()
    {
        while (true)
        {
            yield return new WaitForSeconds(asteroidSpawnTime);
            if (!SuperManager.Instance.GameManager.IsLevelUping)
            {
                var pos = Math.RandomCircle(AsteroidSpawnRadius);
                EZ_Pooling.EZ_PoolManager.Spawn(AsteroidPrefab, pos, Quaternion.identity);
            }

        }

    }
}
