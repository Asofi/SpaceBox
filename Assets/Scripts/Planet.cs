using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Planet : MonoBehaviour, IComparable<Planet>
{

    public float Speed;
    public int Size;
    private int direction;

    public Mesh[] Meshes;
    public Material[] Mats;

    private MeshFilter mMeshFilter;
    private MeshRenderer mMeshRenderer;

    private void Awake()
    {
        mMeshFilter = GetComponent<MeshFilter>();
        mMeshRenderer = GetComponent<MeshRenderer>();
        //EventManager.OnGameStart += OnGameStart;
    }

    private void OnEnable()
    {
        int num = UnityEngine.Random.Range(0, Meshes.Length);
        mMeshFilter.mesh = Meshes[num];
        mMeshRenderer.material = Mats[num];

        Speed = UnityEngine.Random.Range(50, 100);
        direction = UnityEngine.Random.value < 0.5 ? 1 : -1;
    }

    // Update is called once per frame
    void Update () {
        Vector3 newPos = new Vector3(0, Speed * direction * Time.deltaTime, 0);
        transform.Rotate(newPos);
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
