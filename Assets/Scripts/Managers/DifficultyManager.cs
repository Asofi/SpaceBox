using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour {

    public int Level = 0;
    [Space]
    [Header("Orbits")]
    public int WhenChangeOrbitsCount;
    private int orbitsCount = 3;
    public float OrbitSpeedFactor;
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

    [Space]
    [Header("Resize")]
    public Transform[] ObjectsToResize;
    private Vector3[] originSizes;
    private Vector3[] startCubeSizes;
    private float resizeFactor;

    // Use this for initialization
    void Start () {
        EventManager.OnGameStart += OnGameStart;
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

        ChangeSizes();
        
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
    }

    public float GetAsteroidSpeed()
    {
        return Random.Range(MinAsteroidSpeed, MaxAsteroidSpeed);
    }

    public float GetAsteroidSpawnTime()
    {
        return asteroidSpawnTime;
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
}
