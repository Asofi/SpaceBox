using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZ_Pooling;

public class GameManager : MonoBehaviour {
    public Transform AsteroidPrefab;
    public float TimeBetweenSpawnAsteroids = 5f;
    public float AsteroidSpawnRadius;
    public Transform OrbitPrefab;
    public Transform PlanetPool;
    public Transform Sun;
    public Orbit StartOrbitPrefab;
    public Orbit StartOrbit;
    public List<Planet> Planets;
    public List<Orbit> Orbits;
    public int OrbitsCount;
    public int CurPlanetCount;
    public float MinOrbitRadius;
    public float MaxRadius;
    public float DistanceToFirstOrbit;
    public float[] Radiuses;

    private float startCamSize = 70;
    private float curCamSize;
    

    public bool isRemoving = false;


	// Use this for initialization
	void Awake () {
        MinOrbitRadius = 10 + DistanceToFirstOrbit;
	}

    void Start()
    {
        EventManager.OnGameStart += OnGameStart;
        EventManager.OnGameOver += OnGameOver;
        EventManager.OnAddScore += OnAddScore;
        EventManager.OnLevelUp += OnLevelUp;
        StartOrbit = Instantiate(StartOrbitPrefab);
        Orbits.Add(StartOrbit);
        startCamSize = 55 - ((Planets.Count - OrbitsCount) * 8);
        curCamSize = startCamSize;
    }

    IEnumerator SpawnAsteroids()
    {
        while (true)
        {
            yield return new WaitForSeconds(TimeBetweenSpawnAsteroids);
            var pos = Math.RandomCircle(AsteroidSpawnRadius);
            EZ_PoolManager.Spawn(AsteroidPrefab, pos, Quaternion.identity);
        }

    }
    IEnumerator MoveOrbits;

    #region OrbitsControll

    void AddOrbits()
    {
        StartOrbit.DistanceToNext = Radiuses[0] - StartOrbit.Radius;
        Orbit prevOrbit = StartOrbit;

        for(int i = 0; i< OrbitsCount; i ++)
        {

            float radius;
            Transform orbit = Instantiate(OrbitPrefab);
            Orbit orbitScript = orbit.GetComponent<Orbit>();

            orbitScript.Planet = GetPlanet();
            var planet = orbitScript.Planet;
            planet.transform.SetParent(orbit);
            planet.SetActive(true);

            if (i == 0)
            {
                radius = prevOrbit.Radius + orbitScript.Planet.GetComponent<Planet>().Size * 2;

            }
            else
                radius = prevOrbit.Radius + prevOrbit.Planet.GetComponent<Planet>().Size + 1.5f * orbitScript.Planet.GetComponent<Planet>().Size * 2f; 


            Orbits.Add(orbitScript);
            orbitScript.Radius = radius;
            Radiuses[i] = radius;
            orbitScript.OrbitNum = i + 1;

            if (prevOrbit == orbitScript)
            print(radius - prevOrbit.Radius);
                orbitScript.DistanceToPrev = radius - prevOrbit.Radius;
                prevOrbit.DistanceToNext = orbitScript.DistanceToPrev;

            orbitScript.DrawOrbit();
            MaxRadius = radius;


            prevOrbit = orbitScript;
        }
    }

    public void RemoveOrbit(int orbitNum)
    {
        StopAllCoroutines();

        if (orbitNum != 0 && orbitNum != Orbits.Count-1)
        {
            float buffNext = Orbits[orbitNum - 1].DistanceToNext;
            Orbits[orbitNum + 1].DistanceToPrev = buffNext;
        }

        isRemoving = true;
        var orbit = Orbits[orbitNum];
        if(orbit.Planet != null)
        {
            orbit.Planet.transform.SetParent(PlanetPool);
            orbit.Planet.SetActive(false);
            Planets.Add(orbit.Planet.GetComponent<Planet>());
        }
        var removedOrbitsDistToNext = orbit.DistanceToNext;
        var removedOrbitsDistToPrev = orbit.DistanceToPrev;
        var removedRad = orbit.Radius;
        Orbits.RemoveAt(orbitNum);
        orbit.StopAllCoroutines();
        Destroy(orbit.gameObject);
        if (SuperManager.Instance.Player.curOrbitNum >= orbitNum)
            SuperManager.Instance.Player.curOrbitNum--;

        if(zoomCamera != null)
            StopCoroutine(zoomCamera);

        zoomCamera = ZoomCamera(2f);
        StartCoroutine(zoomCamera);

        for (int i = orbitNum; i < Orbits.Count; i++)
        {
            if(orbitNum == 0 || removedOrbitsDistToPrev > Orbits[orbitNum].Planet.GetComponent<Planet>().Size * 2)
            {
                //print("removed normally");
                Orbits[i].StartMovingCoroutine(Orbits[i].Radius - removedOrbitsDistToNext);
                //StartCoroutine(Orbits[i].StartMoveOrbits(Orbits[i].Radius - removedOrbitsDistToNext));
            }
            else
            {
                //print("added " +Orbits[orbitNum].Planet.GetComponent<Planet>().Size * 2);
                Orbits[i].StartMovingCoroutine(Orbits[i].Radius - removedOrbitsDistToNext + Orbits[orbitNum].Planet.GetComponent<Planet>().Size * 2);
                //StartCoroutine(Orbits[i].StartMoveOrbits(Orbits[i].Radius - removedOrbitsDistToNext + Orbits[orbitNum].Planet.GetComponent<Planet>().Size));

            }
        }
    }

    public GameObject GetPlanet()
    {
        if (Planets.Count > 0)
        {
            var border = Planets.Count - (5 - OrbitsCount);
            var num = Random.Range(0, border);
            var buff = Planets[num];
            Planets.RemoveAt(num);
            return buff.gameObject;
        }
        return null;
    }

    #endregion

    #region CameraFuncs
    IEnumerator zoomCamera;
    IEnumerator ZoomCamera(float time)
    {
        Camera cam = Camera.main;
        float startSize = cam.orthographicSize;
        curCamSize -= 4;
        float targetSize = curCamSize;

        float t = 0;
        do
        {
            float newSize = Mathf.Lerp(startSize, targetSize, t);
            cam.orthographicSize = newSize;
            t += 1 / time * Time.deltaTime;
            if (t > 1)
                t = 1;
            yield return null;
        } while (t < 1);

        cam.orthographicSize = targetSize;
    }
    #endregion

    #region EventFuncs
    void OnAddScore()
    {
        if (CurPlanetCount > 0)
            CurPlanetCount--;
        if(CurPlanetCount == 0)
            EventManager.LevelUp();
    }

    void OnLevelUp()
    {
        StopAllCoroutines();
        StartCoroutine(SpawnAsteroids());

        StartOrbit = Orbits[0];
        StartOrbit.Planet.transform.SetParent(PlanetPool);
        StartOrbit.Planet.SetActive(false);
        Planets.Add(StartOrbit.Planet.GetComponent<Planet>());
        StartOrbit.Planet = null;

        Planets.Sort();
        print("Level Up");
        if (StartOrbit == null)
        {
            StartOrbit = Instantiate(StartOrbitPrefab);
            Orbits.Add(StartOrbit);
        }
        Camera.main.orthographicSize = startCamSize;
        curCamSize = startCamSize;
        CurPlanetCount = OrbitsCount;
        AddOrbits();
    }

    void OnGameStart()
    {
        if(StartOrbit == null)
        {
            StartOrbit = Instantiate(StartOrbitPrefab);
            Orbits.Add(StartOrbit);
        }
        Camera.main.orthographicSize = startCamSize;
        curCamSize = startCamSize;
        CurPlanetCount = OrbitsCount;
        AddOrbits();
        StartCoroutine(SpawnAsteroids());

    }

    void OnGameOver()
    {
        int count = Orbits.Count;
        for (int i = 0; i< count; i++)
        {
            RemoveOrbit(0);
        }

        Planets.Sort();
        StopAllCoroutines();
    }
    #endregion
}
