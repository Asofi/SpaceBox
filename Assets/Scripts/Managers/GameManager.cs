using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZ_Pooling;

public class GameManager : MonoBehaviour {

    public static Vector4 WorldsSpaceScreenBorders;
    public static float BorderOffset = 50;

    //private float timeBetweenSpawnAsteroids;
    //public float AsteroidSpawnRadius;
    //public Transform AsteroidPrefab;
    public Transform OrbitPrefab;
    public Transform PlanetPool;
    public Transform Sun;
    public Orbit StartOrbitPrefab;
    public Orbit StartOrbit;
    public List<Planet> Planets;
    public List<Orbit> Orbits;
    public List<Orbit> ActiveOrbits;
    public int OrbitsCount;
    public int CurPlanetCount;
    public int CurCrystallsCount = 0;
    public float MinOrbitRadius;
    public float MaxOrbitRadius;
    public float DistanceToFirstOrbit;
    public float[] Radiuses = new float[6];
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


    private bool isLevelUping = false;
    public bool IsLevelUping
    {
        get
        {
            return isLevelUping;
        }

        set
        {
            if (isLevelUping != value)
            {
                isLevelUping = value;
                if (isLevelUping == false)
                    EventManager.TimerStart();
            }
        }
    }

    public bool isFirstSession = true;

    public LayerMask MaskForCamera;



    void Start()
    {

        

        EventManager.OnGameStart += OnGameStart;
        EventManager.OnGameOver += OnGameOver;
        EventManager.OnAddScore += OnAddScore;
        EventManager.OnAddScore += CheckIsLevelCleared;
        EventManager.OnAddCrystall += OnAddCrystall;
        EventManager.OnAddCrystall += CheckIsLevelCleared;
        EventManager.OnLevelUp += OnLevelUp;
        DefineScreenBorders();

        //Radiuses = new float[6];

        //startCamSize = 60 - ((Planets.Count - OrbitsCount) * 8);
        //curCamSize = startCamSize;
        originCamPos = Camera.main.transform.localPosition;



        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Vector2.zero);

        if (Physics.Raycast(ray, out hit, MaskForCamera))
        {
            distToScreenCorner = hit.point.z;
            print(distToScreenCorner);
        }

        MaxOrbitRadius = distToScreenCorner;

        //distToScreenCorner = 100;

        //StartOrbit = Instantiate(StartOrbitPrefab);
        //ActiveOrbits.Add(StartOrbit);

        //timeBetweenSpawnAsteroids = SuperManager.Instance.DifficultyManager.GetAsteroidSpawnTime();

        //var horzExtent = Camera.main.orthographicSize * Screen.width / Screen.height;
        //print(horzExtent);

        Radiuses3 = CalculateRadiuses(3);
        Radiuses4 = CalculateRadiuses(4);
        Radiuses5 = CalculateRadiuses(5);

    }

    static void DefineScreenBorders()
    {
        WorldsSpaceScreenBorders.x = Camera.main.ScreenToWorldPoint(new Vector2(0 + BorderOffset, 0 + BorderOffset)).z;
        WorldsSpaceScreenBorders.y = Camera.main.ScreenToWorldPoint(new Vector2(0 + BorderOffset, 0 + BorderOffset * 1.5f)).x;
        WorldsSpaceScreenBorders.z = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width - BorderOffset, 0)).z;
        WorldsSpaceScreenBorders.w = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height - BorderOffset * 1.5f)).x;
    }

    #region OrbitsControll

    void DrawOrbits(float extraRad)
    {
        ActiveOrbits.Clear();
        if(isFirstSession)
            ActiveOrbits.Add(StartOrbit);
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

        print(OrbitsCount);
        for (int i = 1; i <= OrbitsCount; i++)
        {
            //Orbits[i].DrawOrbit();
            ActivateOrbit(i);
            Orbits[i].MoveOrbit(Radiuses[i] + extraRad);
            Orbits[i].OrbitNum = isFirstSession ? i+1 : i; //Т.к. в начале у нас есть стартовая орбита, а потом она создается с нуля

            Orbits[i].PlanetSpeed = SuperManager.Instance.DifficultyManager.GetOrbitSpeed();
            Orbits[i].Planet = GetPlanet();
            Orbits[i].isContainsPlanet = true;
            var planet = Orbits[i].Planet;
            planet.transform.SetParent(Orbits[i].transform);
            planet.SetActive(true);
            planet.GetComponent<Planet>().Orbit = Orbits[i].OrbitNum;

        }
        isFirstSession = false;

    }

    void MoveOrbits()
    {
        for (int i = 1; i <= OrbitsCount; i++)
        {
            Orbits[i].StartMovingCoroutine(Radiuses[i], false, true, 1);
        }
    }

    void ActivateOrbit(int num)
    {
        print(Orbits[num].name);
        Orbits[num].mLineRenderer.enabled = true;
        Orbits[num].IsActive = true;
        ActiveOrbits.Add(Orbits[num]);
    }

    void DeactivateOrbit(Orbit orb)
    {
        orb.mLineRenderer.enabled = false;
        orb.IsActive = false;
        ActiveOrbits.Remove(orb);
    }

    void AddOrbits_Old(float extraRad)
    {
        Orbit prevOrbit = StartOrbit;
        if (extraRad == 0)
            Radiuses[0] = prevOrbit.Radius;
        print(OrbitsCount);
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

        for (int i = 0; i < OrbitsCount; i++)
        {

            float radius;
            Transform orbit = Instantiate(OrbitPrefab);
            Orbit orbitScript = orbit.GetComponent<Orbit>();
            orbitScript.PlanetSpeed = SuperManager.Instance.DifficultyManager.GetOrbitSpeed();
            orbitScript.Planet = GetPlanet();
            var planet = orbitScript.Planet;
            planet.transform.SetParent(orbit);
            planet.SetActive(true);



            //if (extraRad == 0)
            //{
            //    if (i == 0)
            //    {
            //        radius = prevOrbit.Radius + distBetweenOrbits;
            //    }
            //    else
            //    {
            //        radius = Radiuses[1] + distBetweenOrbits * i;
            //    }
            //}
            //else
            //{

                radius = Radiuses[i+1];
            //}


            Orbits.Add(orbitScript);
            //if (!IsLevelUping)
                //Radiuses[i+1] = radius;

            radius += extraRad;
            orbitScript.Radius = radius;
            orbitScript.OrbitNum = i + 2;
            planet.GetComponent<Planet>().Orbit = orbitScript.OrbitNum;

            orbitScript.DrawOrbit();
            prevOrbit = orbitScript;

            if(extraRad == 0)
                EventManager.TimerStart();
        }
    }

    float[] CalculateRadiuses(int count)
    {
        float[] radiuses = new float[count+1];
        float distBetweenOrbits = (MaxOrbitRadius - MinOrbitRadius) / count;
        for (int i = 0; i <= count; i++)
            radiuses[i] = distBetweenOrbits * i + MinOrbitRadius;
        
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
        foreach(Orbit orb in ActiveOrbits)
        {
            orb.Radius = MinOrbitRadius +  distBetweenOrbits * (orb.OrbitNum - (removedNum < orb.OrbitNum ? 1 : 0));
        }
    }

    public void RemoveOrbit(int orbitNum)
    {
        //StopAllCoroutines();
        distBetweenOrbits = (MaxOrbitRadius - MinOrbitRadius) / (ActiveOrbits.Count);

        var orbit = ActiveOrbits[orbitNum-1];
        if(orbit.Planet != null)
        {
            orbit.Planet.transform.SetParent(PlanetPool);
            orbit.Planet.SetActive(false);
            Planets.Add(orbit.Planet.GetComponent<Planet>());
        }
        DeactivateOrbit(orbit);
        //ActiveOrbits.RemoveAt(orbitNum-1);
        orbit.StopAllCoroutines();
        //Destroy(orbit.gameObject);
        if (SuperManager.Instance.Player.curOrbitNum >= orbitNum)
            SuperManager.Instance.Player.curOrbitNum--;

        if (GameStateManager.GameState == GameStateManager.GameStates.OnGameOver)
            return;

        //if(zoomCamera != null)
        //    StopCoroutine(zoomCamera);

        //zoomCamera = ZoomCamera(2f, curCamSize);
        //StartCoroutine(zoomCamera);

        RedifineRadiuses(orbitNum);

        for (int i = 0; i < ActiveOrbits.Count; i++)
        {
            var change = false;
            if (i >= orbitNum-1)
                change = true;

                ActiveOrbits[i].StartMovingCoroutine(ActiveOrbits[i].Radius, change, false, 4);
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
        IsLevelUping = true;
        StartOrbit = ActiveOrbits[0];
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

        //curCamSize = startCamSize;
        OrbitsCount = SuperManager.Instance.DifficultyManager.GetOrbitsCount();
        distBetweenOrbits = (MaxOrbitRadius - MinOrbitRadius) / OrbitsCount;
        CurPlanetCount = OrbitsCount;
        DrawOrbits(70);
        for (int i = 0; i < Orbits.Count; i++)
        {
            Orbits[i].StartMovingCoroutine(Radiuses[i], false, true, 3);
        }
    }

    void OnGameStart()
    {
        print("game start");
        OrbitsCount = SuperManager.Instance.DifficultyManager.GetOrbitsCount();
        //timeBetweenSpawnAsteroids = SuperManager.Instance.DifficultyManager.GetAsteroidSpawnTime();
        //if (StartOrbit == null)
        //{
        //    StartOrbit = Instantiate(StartOrbitPrefab);
        //    Orbits.Add(StartOrbit);
        //}
        //Camera.main.orthographicSize = startCamSize;
        distBetweenOrbits = (MaxOrbitRadius - MinOrbitRadius) / OrbitsCount;
        startDistBetweenOrbits = distBetweenOrbits;
        curCamSize = startCamSize;
        CurPlanetCount = OrbitsCount;
        DrawOrbits(70);
        MoveOrbits();
        //Debug.Break();
        //StartCoroutine(SpawnAsteroids());

    }

    void OnGameOver()
    {
        CurCrystallsCount = 0;
        int count = ActiveOrbits.Count;
        for (int i = 0; i< count; i++)
        {
            RemoveOrbit(1);
        }

        Planets.Sort();
        StopAllCoroutines();
    }
    #endregion
}
