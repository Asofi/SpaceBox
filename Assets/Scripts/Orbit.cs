using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour {
    [Header("Draw Settings")]
    public float ThetaScale = 0.03f;        //Set lower to add more points
    public Material LineMaterial;
    int size; //Total number of points in circle
    LineRenderer lineRenderer;
    public float Radius;
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
    }

    private void Start()
    {
        

        if (CompareTag("StartOrbit"))
        {
            Radius = SuperManager.Instance.PlanetManager.MinOrbitRadius;
            DrawOrbit();
        }

        if (Planet == null)
        {
            isContainsPlanet = false;
            return;
        }

        //PlanetSize = Vector3.one * SuperManager.Instance.PlanetManager.GetPlanetSize();
        //planet.transform.localPosition = Vector3.right * Radius;
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
            SuperManager.Instance.PlanetManager.RemoveOrbit(OrbitNum);
    }

    public void DrawOrbit()
    {
        float sizeValue = (2.0f * Mathf.PI) / ThetaScale;
        size = (int)sizeValue;
        size++;
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = LineMaterial;
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.widthMultiplier = 3;
        lineRenderer.numPositions = size;

        Vector3 pos;
        float theta = 0f;
        for (int i = 0; i < size; i++)
        {
            theta += (2.0f * Mathf.PI * ThetaScale);
            float x = Radius * Mathf.Cos(theta);
            float z = Radius * Mathf.Sin(theta);
            x += gameObject.transform.position.x;
            z += gameObject.transform.position.z;
            pos = new Vector3(x, 0, z);
            lineRenderer.SetPosition(i, pos);
        }

        if(Planet != null)
        {
            Planet.transform.localPosition = new Vector3(Radius, 0, 0);
        }

        CurRadius = Radius;
    }

    public IEnumerator StartMoveOrbits(float targetRadius)
    {
        if (isContainsPlayer)
        {
            SuperManager.Instance.Player.distanceToNext = DistanceToNext;
            SuperManager.Instance.Player.distanceToPrev = DistanceToPrev;
        }
        Radius = targetRadius;
        OrbitNum--;
        float startPos = CurRadius;
        float t = 0;
        while (t <= 1)
        {
            float newRadius = Mathf.Lerp(CurRadius, targetRadius, t);
            CurRadius = newRadius;
            t += 0.25f * Time.deltaTime;
            print(t);
            if (t > 0.4 && t <= 1)
                t = 1;
            MoveOrbit(CurRadius);
            if (isContainsPlayer)
            {
                SuperManager.Instance.Player.transform.localPosition = new Vector3(0, 0, CurRadius);
                SuperManager.Instance.Player.GetComponent<Player>().CubeOrbitR = CurRadius;
            }
            yield return new WaitForEndOfFrame();
        }
        SuperManager.Instance.PlanetManager.isRemoving = false;
        CurRadius = Radius;
        MoveOrbit(CurRadius);
        //if (isContainsPlayer)
        //    SuperManager.Instance.Player.GetComponent<Player>().CubeOrbitR = CurRadius;
    }


    void MoveOrbit(float radius)
    {
        Vector3 pos;
        float theta = 0f;
        for (int i = 0; i < size; i++)
        {
            theta += (2.0f * Mathf.PI * ThetaScale);
            float x = radius * Mathf.Cos(theta);
            float z = radius * Mathf.Sin(theta);
            x += gameObject.transform.position.x;
            z += gameObject.transform.position.z;
            pos = new Vector3(x, 0, z);
            lineRenderer.SetPosition(i, pos);

            if(Planet != null)
                Planet.transform.localPosition = new Vector3(radius, 0, 0);

        }
    }

    //Vector3 RandomCircle(float radius)
    //{
    //    float ang = Random.value * 360;
    //    Vector3 pos;
    //    pos.x = radius * Mathf.Sin(ang * Mathf.Deg2Rad);
    //    pos.y = 0;
    //    pos.z = radius * Mathf.Cos(ang * Mathf.Deg2Rad);
    //    return pos;
    //}
}
