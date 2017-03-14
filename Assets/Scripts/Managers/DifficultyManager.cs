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

	// Use this for initialization
	void Start () {
        EventManager.OnGameStart += OnGameStart;
        EventManager.OnLevelUp += OnLevelUp;
        originMinSpeed = MinAsteroidSpeed;
        originMaxSpeed = MaxAsteroidSpeed;
        asteroidSpawnTime = MaxAsteroidSpawnTime;
    }

    void OnGameStart()
    {
        Level = 1;
        asteroidSpawnTime = MaxAsteroidSpawnTime;
        MinAsteroidSpeed = originMinSpeed;
        MaxAsteroidSpeed = originMaxSpeed;
        orbitsCount = 3;
        
    }

    void OnLevelUp()
    {
        Level++;
        if (Level % WhenChangeOrbitsCount == 0 && orbitsCount < 5)
            orbitsCount++;
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
}
