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

    public float CameraShakeTime = 1;
    public float ShakeAmount = 5;
    private Vector3 originCamPos;

    private float startCamSize = 70;
    private float curCamSize;

    

    public bool isLevelUping = false;


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
        originCamPos = Camera.main.transform.localPosition;
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

    #region OrbitsControll

    void AddOrbits(float extraRad)
    {
        StartOrbit.DistanceToNext = Radiuses[0] - StartOrbit.Radius;
        Orbit prevOrbit = StartOrbit;

        for (int i = 0; i < OrbitsCount; i++)
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
                radius = /*prevOrbit.Radius*/ Radiuses[i-1] + prevOrbit.Planet.GetComponent<Planet>().Size + 1.5f * orbitScript.Planet.GetComponent<Planet>().Size * 2f;


            Orbits.Add(orbitScript);
            Radiuses[i] = radius;
            if(i == 0)
                orbitScript.DistanceToPrev = radius - prevOrbit.Radius;
            else
                orbitScript.DistanceToPrev = radius - Radiuses[i - 1];
            radius += extraRad;
            orbitScript.Radius = radius;
            orbitScript.OrbitNum = i + 1;

            prevOrbit.DistanceToNext = orbitScript.DistanceToPrev;

            orbitScript.DrawOrbit();
            MaxRadius = radius;


            prevOrbit = orbitScript;
        }
    }

    public void RemoveOrbit(int orbitNum)
    {
        //StopAllCoroutines();

        if (orbitNum != 0 && orbitNum != Orbits.Count-1)
        {
            float buffNext = Orbits[orbitNum - 1].DistanceToNext;
            Orbits[orbitNum + 1].DistanceToPrev = buffNext;
        }

        var orbit = Orbits[orbitNum];
        if(orbit.Planet != null)
        {
            orbit.Planet.transform.SetParent(PlanetPool);
            orbit.Planet.SetActive(false);
            Planets.Add(orbit.Planet.GetComponent<Planet>());
        }
        var removedOrbitsDistToNext = orbit.DistanceToNext;
        var removedOrbitsDistToPrev = orbit.DistanceToPrev;
        //var removedRad = orbit.Radius;
        Orbits.RemoveAt(orbitNum);
        orbit.StopAllCoroutines();
        Destroy(orbit.gameObject);
        if (SuperManager.Instance.Player.curOrbitNum >= orbitNum)
            SuperManager.Instance.Player.curOrbitNum--;

        if (GameStateManager.GameState == GameStateManager.GameStates.OnGameOver)
            return;

        if(zoomCamera != null)
            StopCoroutine(zoomCamera);

        zoomCamera = ZoomCamera(2f, curCamSize);
        StartCoroutine(zoomCamera);

        for (int i = orbitNum; i < Orbits.Count; i++)
        {
            if(orbitNum == 0)
            {
                if(i == 0)
                    Orbits[i].StartMovingCoroutine(MinOrbitRadius, true, 4);
                else
                    Orbits[i].StartMovingCoroutine(Orbits[i].Radius - removedOrbitsDistToNext, true, 4);
            }
            else
            {
                Orbits[i].StartMovingCoroutine(Orbits[i].Radius - removedOrbitsDistToNext + Orbits[orbitNum].Planet.GetComponent<Planet>().Size * 2, true, 4);

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
    IEnumerator ZoomCamera(float time, float targetSize)
    {
        print("Start Zooming");
        Camera cam = Camera.main;
        float startSize = cam.orthographicSize;
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
        print("End Zooming");
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
    }
    #endregion

    #region EventFuncs
    void OnAddScore()
    {
        if (CurPlanetCount > 0)
            CurPlanetCount--;
        if (CurPlanetCount == 0)
            StartCoroutine(ChangeLevel());
            //EventManager.LevelUp();

    }

    IEnumerator ChangeLevel()
    {

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
        //StopAllCoroutines();
        //StartCoroutine(SpawnAsteroids());
        isLevelUping = true;
        StartOrbit = Orbits[0];
        StartOrbit.Planet.transform.SetParent(PlanetPool);
        StartOrbit.Planet.SetActive(false);
        Planets.Add(StartOrbit.Planet.GetComponent<Planet>());
        StartOrbit.Planet = null;

        Planets.Sort();
        if (StartOrbit == null)
        {
            StartOrbit = Instantiate(StartOrbitPrefab);
            Orbits.Add(StartOrbit);
        }
        //Camera.main.orthographicSize = startCamSize;

        if (zoomCamera != null)
            StopCoroutine(zoomCamera);
        zoomCamera = ZoomCamera(3f, startCamSize);
        StartCoroutine(zoomCamera);

        curCamSize = startCamSize;
        CurPlanetCount = OrbitsCount;
        AddOrbits(70);

        for (int i = 1; i < Orbits.Count; i++)
        {
            print(Orbits.Count);
            print(Radiuses.Length);
            Orbits[i].StartMovingCoroutine(Radiuses[i-1], false, 32);
        }
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
        AddOrbits(0);
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
