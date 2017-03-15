using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Planet : MonoBehaviour, IComparable<Planet>
{

    public float Speed;
    public int Size;
    private int direction;
    public int Orbit;

    public Mesh[] Meshes;
    public Material[] Mats;

    Orbit ParentOrbit;
    public Transform Graphics;
    public MeshFilter mMeshFilter;
    public MeshRenderer mMeshRenderer;

    private void Awake()
    {
        //mMeshFilter = GetComponent<MeshFilter>();
        //mMeshRenderer = GetComponent<MeshRenderer>();
        //EventManager.OnGameStart += OnGameStart;
    }

    private void OnEnable()
    {
        ParentOrbit = transform.parent.GetComponent<Orbit>();
        int num = UnityEngine.Random.Range(0, Meshes.Length);
        mMeshFilter.mesh = Meshes[num];
        mMeshRenderer.material = Mats[num];

        Speed = UnityEngine.Random.Range(60, 80);
        direction = UnityEngine.Random.value < 0.5 ? 1 : -1;
    }

    // Update is called once per frame
    void Update () {
        Vector3 newPos = new Vector3(0, -ParentOrbit.PlanetSpeed * ParentOrbit.PlanetDirection * Time.deltaTime, 0);
        transform.Rotate(newPos, Space.World);
        Graphics.localEulerAngles = new Vector3(0, Graphics.localEulerAngles.y + Speed * Time.deltaTime, 0);
        //Vector3 newPos = new Vector3(0, Speed * direction * Time.deltaTime, 0);
        //transform.Rotate(newPos, Space.World);
    }

    public int CompareTo(Planet other)
    {
        if (Size > other.Size)
            return 1;
        if (Size < other.Size)
            return -1;
        else
            return 0;
    }
}
