using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZ_Pooling;

public class GameManager : MonoBehaviour {
    private float timeBetweenSpawnAsteroids;
    public float AsteroidSpawnRadius;
    public Transform AsteroidPrefab;
    public Transform OrbitPrefab;
    public Transform PlanetPool;
    public Transform Sun;
    public Orbit StartOrbitPrefab;
    public Orbit StartOrbit;
    public List<Planet> Planets;
    public List<Orbit> Orbits;
    public int OrbitsCount;
    [HideInInspector]
    public int CurPlanetCount;
    [HideInInspector]
    public int CurCrystallsCount = 0;
    public float MinOrbitRadius;
    public float DistanceToFirstOrbit;
    private float[] Radiuses;
    public float[] Radiuses3;
    public float[] Radiuses4;
    public float[] Radiuses5;

    public float CameraShakeTime = 1;
    public float ShakeAmount = 5;
    private Vector3 originCamPos;

    private float startCamSize = 60;
    private float curCamSize;

    private float distToScreenCorner;
    private float distBetweenOrbits;
    private float startDistBetweenOrbits;
    

    public bool isLevelUping = false;
    public bool isFirstSession = true;


    void Start()
    {
        EventManager.OnGameStart += OnGameStart;
        EventManager.OnGameOver += OnGameOver;
        EventManager.OnAddScore += OnAddScore;
        EventManager.OnAddScore += CheckIsLevelCleared;
        EventManager.OnAddCrystall += OnAddCrystall;
        EventManager.OnAddCrystall += CheckIsLevelCleared;
        EventManager.OnLevelUp += OnLevelUp;

        Radiuses = new float[6];

        startCamSize = 60 - ((Planets.Count - OrbitsCount) * 8);
        curCamSize = startCamSize;
        originCamPos = Camera.main.transform.localPosition;

        distToScreenCorner = Camera.main.ScreenToWorldPoint(Vector2.zero).z;
        //distToScreenCorner = 100;

        StartOrbit = Instantiate(StartOrbitPrefab);
        Orbits.Add(StartOrbit);

        timeBetweenSpawnAsteroids = SuperManager.Instance.DifficultyManager.GetAsteroidSpawnTime();

        Radiuses3 = CalculateRadiuses(3);
        Radiuses4 = CalculateRadiuses(4);
        Radiuses5 = CalculateRadiuses(5);
    }

    IEnumerator SpawnAsteroids()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenSpawnAsteroids);
            if (!isLevelUping)
            {
                var pos = Math.RandomCircle(AsteroidSpawnRadius);
                EZ_PoolManager.Spawn(AsteroidPrefab, pos, Quaternion.identity);
            }

        }

    }

    #region OrbitsControll

    void AddOrbits(float extraRad)
    {
        Orbit prevOrbit = StartOrbit;
        if (extraRad == 0)
            Radiuses[0] = prevOrbit.Radius;
        for (int i = 0; i < OrbitsCount; i++)
        {

            float radius;
            Transform orbit = Instantiate(OrbitPrefab);
            Orbit orbitScript = orbit.GetComponent<Orbit>();

            orbitScript.Planet = GetPlanet();
            var planet = orbitScript.Planet;
            planet.transform.SetParent(orbit);
            planet.SetActive(true);
            if (extraRad == -0)
            {
                if (i == 0)
                {
                    radius = prevOrbit.Radius + distBetweenOrbits;
                }
                else
                {
                    radius = Radiuses[1] + distBetweenOrbits * i;
                }
              }
            else
            {
                switch (OrbitsCount)
                {
                    case 3:
                        RewriteRadiuses(Radiuses3);
                        //Radiuses = Radiuses3;
                        break;
                    case 4:
                        RewriteRadiuses(Radiuses4);
                        //Radiuses = Radiuses4;
                        break;
                    case 5:
                        RewriteRadiuses(Radiuses5);
                        //Radiuses = Radiuses5;
                        break;
                }
                radius = Radiuses[i];
            }


            Orbits.Add(orbitScript);
            if (!isLevelUping)
                Radiuses[i+1] = radius;

            radius += extraRad;
            orbitScript.Radius = radius;
            orbitScript.OrbitNum = i + 2;
            planet.GetComponent<Planet>().Orbit = orbitScript.OrbitNum;

            orbitScript.DrawOrbit();
            prevOrbit = orbitScript;
        }
    }

    float[] CalculateRadiuses(int count)
    {
        float[] radiuses = new float[count+1];
        float distBetweenOrbits = (distToScreenCorner - MinOrbitRadius) / count;
        for (int i = 0; i <= count; i++)
            radiuses[i] = MinOrbitRadius + distBetweenOrbits * i;
        
        return radiuses;
    }
    void RewriteRadiuses(float[] rad)
    {
        for (int i = 0; i < rad.Length; i++)
        {
            Radiuses[i] = rad[i];
        }
    }

    public void RedifineRadiuses(int removedNum)
    {
        foreach(Orbit orb in Orbits)
        {
            orb.Radius = MinOrbitRadius +  distBetweenOrbits * (orb.OrbitNum - (removedNum < orb.OrbitNum ? 1 : 0));
        }
    }

    public void RemoveOrbit(int orbitNum)
    {
        //StopAllCoroutines();
        distBetweenOrbits = (distToScreenCorner - MinOrbitRadius) / (Orbits.Count);

        var orbit = Orbits[orbitNum-1];
        if(orbit.Planet != null)
        {
            orbit.Planet.transform.SetParent(PlanetPool);
            orbit.Planet.SetActive(false);
            Planets.Add(orbit.Planet.GetComponent<Planet>());
        }
        Orbits.RemoveAt(orbitNum-1);
        orbit.StopAllCoroutines();
        Destroy(orbit.gameObject);
        if (SuperManager.Instance.Player.curOrbitNum >= orbitNum)
            SuperManager.Instance.Player.curOrbitNum--;

        if (GameStateManager.GameState == GameStateManager.GameStates.OnGameOver)
            return;

        //if(zoomCamera != null)
        //    StopCoroutine(zoomCamera);

        //zoomCamera = ZoomCamera(2f, curCamSize);
        //StartCoroutine(zoomCamera);

        RedifineRadiuses(orbitNum);

        for (int i = 0; i < Orbits.Count; i++)
        {
            var change = false;
            if (i >= orbitNum-1)
                change = true;

                Orbits[i].StartMovingCoroutine(Orbits[i].Radius, change, false, 4);
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
    IEnumerator ZoomCamera(float time, float targetSize)
    {
        Camera cam = Camera.main;
        float startSize = cam.orthographicSize;
        if (Orbits.Count == 2)
            curCamSize -= 15;
        else 
            curCamSize -= 4;

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

    private IEnumerator cameraShake;
    private IEnumerator CameraShake()
    {
        float shakeTime = CameraShakeTime;
        var mCam = Camera.main;
        while (shakeTime >= 0)
        {
            mCam.transform.localPosition = new Vector3(mCam.transform.localPosition.x,
                                                        Random.Range(originCamPos.y - ShakeAmount, originCamPos.y + ShakeAmount),
                                                        Random.Range(originCamPos.z - ShakeAmount, originCamPos.z + ShakeAmount));
            shakeTime -= Time.deltaTime;
            yield return null;
        }
        mCam.transform.localPosition = originCamPos;

        //if (zoomCamera != null)
        //    StopCoroutine(zoomCamera);
        //zoomCamera = ZoomCamera(2f, startCamSize);
        ////yield return new WaitForSeconds(0.2f);
        //StartCoroutine(zoomCamera);

    }
    #endregion

    #region EventFuncs

    void OnAddCrystall()
    {
        CurCrystallsCount--;
    }

    void OnAddScore()
    {
        if (CurPlanetCount > 0)
            CurPlanetCount--;
    }

    void CheckIsLevelCleared()
    {
        if (CurPlanetCount == 0 && CurCrystallsCount == 0)
            StartCoroutine(ChangeLevel());
    }

    IEnumerator ChangeLevel()
    {
        isLevelUping = true;
        StartOrbit = Orbits[0];
        yield return new WaitForSeconds(1f);
        if (cameraShake != null)
            StopCoroutine(cameraShake);
        cameraShake = CameraShake();
        StartCoroutine(cameraShake);
        yield return new WaitForSeconds(1f);
        EventManager.LevelUp();
    }

    void OnLevelUp()
    {
        ///Difficulty
        timeBetweenSpawnAsteroids = SuperManager.Instance.DifficultyManager.GetAsteroidSpawnTime();

        ///Orbits
        StartOrbit.Planet.transform.SetParent(PlanetPool);
        StartOrbit.Planet.SetActive(false);
        Planets.Add(StartOrbit.Planet.GetComponent<Planet>());
        StartOrbit.Planet = null;

        CurCrystallsCount = 0;

        Planets.Sort();
        //if (StartOrbit == null)
        //{
        //    StartOrbit = Instantiate(StartOrbitPrefab);
        //    Orbits.Add(StartOrbit);
        //}
        //Camera.main.orthographicSize = startCamSize;

        curCamSize = startCamSize;
        distBetweenOrbits = startDistBetweenOrbits;
        OrbitsCount = SuperManager.Instance.DifficultyManager.GetOrbitsCount();
        CurPlanetCount = OrbitsCount;
        AddOrbits(70);
        for (int i = 0; i < Orbits.Count; i++)
        {
            Orbits[i].StartMovingCoroutine(Radiuses[i], false, true, 3);
        }
    }

    void OnGameStart()
    {
        print("game start");
        OrbitsCount = SuperManager.Instance.DifficultyManager.GetOrbitsCount();
        timeBetweenSpawnAsteroids = SuperManager.Instance.DifficultyManager.GetAsteroidSpawnTime();
        if (StartOrbit == null)
        {
            StartOrbit = Instantiate(StartOrbitPrefab);
            Orbits.Add(StartOrbit);
        }
        //Camera.main.orthographicSize = startCamSize;
        distBetweenOrbits = (distToScreenCorner - 10) / OrbitsCount ;
        startDistBetweenOrbits = distBetweenOrbits;
        curCamSize = startCamSize;
        CurPlanetCount = OrbitsCount;
        AddOrbits(0);
        StartCoroutine(SpawnAsteroids());

    }

    void OnGameOver()
    {
        CurCrystallsCount = 0;
        int count = Orbits.Count;
        for (int i = 0; i< count; i++)
        {
            RemoveOrbit(1);
        }

        Planets.Sort();
        StopAllCoroutines();
    }
    #endregion
}
