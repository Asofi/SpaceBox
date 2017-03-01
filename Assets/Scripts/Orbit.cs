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
    public bool isContainsPlanet = true;
    public bool isContainsPlayer = false;
    public int OrbitNum;
    public float DistanceToNext;
    public float DistanceToPrev;

    void Awake()
    {
        //planet = transform.FindChild("Planet");
        PlanetSpeed = Random.Range(5, 50);
        PlanetDirection = Random.value < 0.5 ? 1 : -1;
        Radius = SuperManager.Instance.GameManager.MinOrbitRadius;
    }

    private void Start()
    {
        //EventManager.OnGameOver += StopMovingOrbit;
        //EventManager.OnLevelUp += StopMovingOrbit;

        if (CompareTag("StartOrbit"))
        {
            DrawOrbit();
        }

        if (Planet == null)
        {
            isContainsPlanet = false;
            return;
        }

        transform.Rotate(Vector3.up, Random.Range(0, 360));
    }

    private void Update()
    {
        if (isContainsPlanet)
        {
            Vector3 newPos = new Vector3(0, PlanetSpeed * PlanetDirection * Time.deltaTime, 0);
            transform.Rotate(newPos);
        }
    }

    private void FixedUpdate()
    {
        if (!isContainsPlanet && !isContainsPlayer)
            SuperManager.Instance.GameManager.RemoveOrbit(OrbitNum);
    }

    public void DrawOrbit()
    {

        //float sizeValue = (int)((1f / ThetaScale) + 2f);
        //size = (int)sizeValue;
        //size++;
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = LineMaterial;
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.widthMultiplier = 13;
        lineRenderer.numPositions = Segments + 1;

        float x;
        float y = 0f;
        float z;

        float angle = 20f;

        for (int i = 0; i < (Segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * Radius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * Radius;

            lineRenderer.SetPosition(i, new Vector3(x, y, z));

            angle += (360f / Segments);
        }

        //Vector3 pos;
        //float theta = 0f;
        //for (int i = 0; i < size; i++)
        //{
        //    theta += (2.0f * Mathf.PI * ThetaScale);
        //    float x = Radius * Mathf.Cos(theta);
        //    float z = Radius * Mathf.Sin(theta);
        //    x += gameObject.transform.position.x;
        //    z += gameObject.transform.position.z;
        //    pos = new Vector3(x, 0, z);
        //    lineRenderer.SetPosition(i, pos);
        //}

        if (Planet != null)
        {
            Planet.transform.localPosition = new Vector3(Radius, 0, 0);
        }

        CurRadius = Radius;
    }

    public void StartMovingCoroutine(float targetRad, bool changeOrbitNum, float time)
    {
        if (startMovingOrbit != null)
            StopCoroutine(startMovingOrbit);
        startMovingOrbit = StartMoveOrbits(targetRad, changeOrbitNum, time);
        StartCoroutine(startMovingOrbit);
    }

    private IEnumerator startMovingOrbit;
    private IEnumerator StartMoveOrbits(float targetRadius, bool changeOrbitNum, float time)
    {
        Radius = targetRadius;
        if(changeOrbitNum)
            OrbitNum--;
        float t = 0;
        while (t < 1)
        {
            float newRadius = Mathf.Lerp(CurRadius, targetRadius, t);
            CurRadius = newRadius;
            t += 1/time * Time.deltaTime;
            if(t > 0.05)
                SuperManager.Instance.GameManager.isLevelUping = false;
            if (t > 0.4 && t <= 1)
                t = 1;
            MoveOrbit(CurRadius);


            if (isContainsPlayer && SuperManager.Instance.Player.IsOnOrbit)
            {
                SuperManager.Instance.Player.transform.localPosition = new Vector3(0, 0, CurRadius);
                SuperManager.Instance.Player.GetComponent<Player>().CubeOrbitR = CurRadius;
            }
            yield return new WaitForEndOfFrame();
        }

        CurRadius = Radius;
        if (isContainsPlayer && SuperManager.Instance.Player.IsOnOrbit)
        {
            SuperManager.Instance.Player.transform.localPosition = new Vector3(0, 0, CurRadius);
        }
        //MoveOrbit(CurRadius);
    }


    void MoveOrbit(float radius)
    {
        float x;
        float y = 0f;
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
    }

    void StopMovingOrbit()
    {
        StopAllCoroutines();
    }
}
