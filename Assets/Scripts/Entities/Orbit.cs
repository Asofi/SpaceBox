using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour {
    [Header("Draw Settings")]
    public float ThetaScale = 0.03f;        //Set lower to add more points
    public Material LineMaterial;
    public int Segments; //Total number of points in circle
    public float Radius;
    LineRenderer lineRenderer;
    public float CurRadius;

    [Space]
    [Header("Planet Settings")]
    public Vector3 PlanetSize;
    public float PlanetSpeed = 5f;
    public int PlanetDirection = 1;
    public GameObject Planet;
    public Transform CrystallPivot;
    public Transform Crystall;
    public bool isContainsPlanet = true;
    public bool isContainsPlayer = false;
    public bool isContainsCrystall = false;
    public int OrbitNum;

    private float orbitAngle;

    public float CrystallSpawnChance = 0.3f;

    void Awake()
    {
        //PlanetSpeed = Random.Range(10, 35);
        PlanetDirection = /*Random.value < 0.5 ? 1 : -1;*/ -1;
        Radius = SuperManager.Instance.GameManager.MinOrbitRadius;
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {

        if (CompareTag("StartOrbit"))
        {
            DrawOrbit();
        }

        //if (Planet == null)
        //{
        //    isContainsPlanet = false;
        //    return;
        //}

        //TEMP COMMENT

        //orbitAngle = Random.Range(0, 360);
        //transform.Rotate(Vector3.up, orbitAngle);
    }

    private void Update()
    {
        if (isContainsPlanet || isContainsCrystall)
        {
            Vector3 newPos = new Vector3(0, PlanetSpeed * PlanetDirection * Time.deltaTime, 0);
            transform.Rotate(newPos);
        }
    }

    private void FixedUpdate()
    {
        if (!isContainsPlanet && !isContainsPlayer && !isContainsCrystall)
            SuperManager.Instance.GameManager.RemoveOrbit(OrbitNum);
    }

    public void DrawOrbit()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = LineMaterial;
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.widthMultiplier = 13;
        lineRenderer.numPositions = Segments + 1;

        float x;
        float y = -10f;
        float z;

        float angle = 20f;

        for (int i = 0; i < (Segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * Radius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * Radius;

            lineRenderer.SetPosition(i, new Vector3(x, y, z));

            angle += (360f / Segments);
        }

        if (Planet != null)
        {
            Planet.transform.localPosition = new Vector3(Radius, 0, 0);
        }

        isContainsCrystall = false;
        if (CrystallPivot != null)
        {
            if(Random.value <= CrystallSpawnChance)
            {
                SpawnCrystall();
            }

        }

        CurRadius = Radius;
    }

    private void SpawnCrystall()
    {
        float crystallAngle = Random.Range(30, 330);

        CrystallPivot.Rotate(Vector3.up, crystallAngle);
        CrystallPivot.FindChild("Crystall").localPosition = new Vector3(Radius, 0, 0);
        //CrystallPivot.SetParent(transform);
        CrystallPivot.gameObject.SetActive(true);
        isContainsCrystall = true;

        SuperManager.Instance.GameManager.CurCrystallsCount++;
    }

    public void StartMovingCoroutine(float targetRad, bool changeOrbitNum, bool isStraight, float time)
    {
        if (startMovingOrbit != null)
            StopCoroutine(startMovingOrbit);
        startMovingOrbit = StartMoveOrbits(targetRad, changeOrbitNum, isStraight, time);
        StartCoroutine(startMovingOrbit);
    }

    private IEnumerator startMovingOrbit;
    private IEnumerator StartMoveOrbits(float targetRadius, bool changeOrbitNum, bool isStraight, float time)
    {
        var oldRad = CurRadius;
        Radius = targetRadius;
        if (changeOrbitNum)
        {
            OrbitNum--;
            Planet.GetComponent<Planet>().Orbit = OrbitNum;
        }
        float t = 0;
        while (t < 1)
        {
            float newRadius;
            if (!isStraight)
            {
                newRadius = Mathf.Lerp(CurRadius, targetRadius, t);
                CurRadius = newRadius;
                t += 1 / time * Time.deltaTime;
                if (t > 0.4 && t <= 1)
                    t = 1;
                MoveOrbit(CurRadius);
            }
            else
            {
                newRadius = Mathf.Lerp(oldRad, targetRadius, t);
                CurRadius = newRadius;
                t += 1 / time * Time.deltaTime;
                MoveOrbit(CurRadius);
            }
           


            if (isContainsPlayer && SuperManager.Instance.Player.IsOnOrbit)
            {
                SuperManager.Instance.Player.transform.localPosition = new Vector3(0, 0, CurRadius);
                SuperManager.Instance.Player.GetComponent<Player>().CubeOrbitR = CurRadius;
            }
            yield return new WaitForEndOfFrame();
        }
        if (SuperManager.Instance.GameManager.IsLevelUping && isStraight)
        {
            SuperManager.Instance.GameManager.IsLevelUping = false;

        }
        CurRadius = Radius;
        if (isContainsPlayer && SuperManager.Instance.Player.IsOnOrbit)
        {
            SuperManager.Instance.Player.transform.localPosition = new Vector3(0, 0, CurRadius);
        }
    }


    public void MoveOrbit(float radius)
    {
        float x;
        float y = -10f;
        float z;

        float angle = 20f;

        for (int i = 0; i < (Segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            lineRenderer.SetPosition(i, new Vector3(x, y, z));

            angle += (360f / Segments);
        }

        if (Planet != null)
            Planet.transform.localPosition = new Vector3(radius, 0, 0);
        if (Crystall != null)
            Crystall.localPosition = new Vector3(radius, 0, 0);
    }

    void StopMovingOrbit()
    {
        StopAllCoroutines();
    }
}
