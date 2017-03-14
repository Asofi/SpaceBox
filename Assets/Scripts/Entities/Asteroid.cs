using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour {

    public float Speed;
    public int Size;


    public Mesh[] Meshes;

    private MeshFilter mMeshFilter;
    private MeshRenderer mRenderer;
    private Rigidbody mRigid;
    public ParticleSystem Expl;
    private bool deadPlaying = false;

    private void Awake()
    {
        EventManager.OnLevelUp += DestroyAsteroid;
        EventManager.OnGameOver += DestroyAsteroid;

        mMeshFilter = transform.FindChild("Graphics").GetComponent<MeshFilter>();
        mRenderer = transform.FindChild("Graphics").GetComponent<MeshRenderer>();
        mRigid = GetComponent<Rigidbody>();

    }

    void OnSpawned()
    {
        mRenderer.enabled = true;
        GetComponent<Collider>().enabled = true;
        int num = Random.Range(0, Meshes.Length);
        mMeshFilter.mesh = Meshes[num];
        transform.LookAt(SuperManager.Instance.GameManager.Sun);

        Speed = SuperManager.Instance.DifficultyManager.GetAsteroidSpeed();
        mRigid.AddForce(transform.forward * 100 * Speed);

    }

    private void OnCollisionEnter(Collision collision)
    {
        DestroyAsteroid();
    }

    IEnumerator delayedDespawn;
    IEnumerator DelayedDespawn(float time)
    {
        deadPlaying = true;
        Expl.Play();
        mRenderer.enabled = false;
        GetComponent<Collider>().enabled = false;
        mRigid.velocity = Vector3.zero;
        yield return new WaitForSeconds(time);
        EZ_Pooling.EZ_PoolManager.Despawn(transform);
        deadPlaying = false;
    }

    void DestroyAsteroid()
    {
        if (!deadPlaying && gameObject.activeSelf)
            StartCoroutine(DelayedDespawn(4));
    }


}
