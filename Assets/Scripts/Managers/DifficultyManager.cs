using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("FrontSectors")]
    public Vector2[] FrontSectors;
    [Range(0, 1)] public float MinChanceToStartFront = 0f;
    [Range(0, 1)] public float MaxChanceToStartFront = 0.3f;
    private float curChanceToStartFront;
    public float ChanceStep = 0.01f;

    public float FrontTimeStep = 0.1f;
    public float MinFrontTime = 1;
    public float MaxFrontTime = 5;
    private float curFrontTime;

    public Image[] FrontIndicators;

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
        orbitsCount = 5;
        curFrontTime = MinFrontTime;
        curChanceToStartFront = MinChanceToStartFront;

        orbitCurSpeed = OrbitMinSpeed;

        ChangeSizes();

        StartCoroutine(SpawnAsteroids());
        StartCoroutine(EventsTimer());
        
    }

    void OnGameOver()
    {
        StopAllCoroutines();
        foreach (Image img in FrontIndicators)
            img.gameObject.SetActive(false);
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

        curFrontTime = Mathf.Clamp(curFrontTime + FrontTimeStep, MinFrontTime, MaxFrontTime);

        curChanceToStartFront = Mathf.Clamp(curChanceToStartFront + ChanceStep, MinChanceToStartFront, MaxChanceToStartFront);
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

    IEnumerator AsteroidFront(float time, int frontSector)
    {
        FrontIndicators[frontSector].gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        FrontIndicators[frontSector].gameObject.SetActive(false);

        float t = 0;
        while (t < time)
        {
            t += 0.2f;
            yield return new WaitForSeconds(0.2f);
            if (!SuperManager.Instance.GameManager.IsLevelUping)
            {
                var pos = Math.RandomCircle(AsteroidSpawnRadius, FrontSectors[frontSector]);
                EZ_Pooling.EZ_PoolManager.Spawn(AsteroidPrefab, pos, Quaternion.identity);
            }

        }
    }

    IEnumerator EventsTimer()
    {
        while (true)
        {
            var num = Random.Range(0, 4);
            yield return new WaitForSeconds(2);
            if (Random.value < curChanceToStartFront && !SuperManager.Instance.GameManager.IsLevelUping)
                StartCoroutine(AsteroidFront(curFrontTime, num));
        }
        
    }
}
