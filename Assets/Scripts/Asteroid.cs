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

    private void Awake()
    {
        mMeshFilter = transform.FindChild("Graphics").GetComponent<MeshFilter>();
        mRenderer = transform.FindChild("Graphics").GetComponent<MeshRenderer>();
        mRigid = GetComponent<Rigidbody>();

    }

    void OnSpawned()
    {
        mRenderer.enabled = true;
        int num = Random.Range(0, Meshes.Length);
        mMeshFilter.mesh = Meshes[num];
        transform.LookAt(SuperManager.Instance.GameManager.Sun);

        Speed = Random.Range(10, 15);
        mRigid.AddForce(transform.forward * 100 * Speed);

    }

    private void OnCollisionEnter(Collision collision)
    {
        Expl.Play();
        mRenderer.enabled = false;
        StartCoroutine(DelayedDespawn(4));
    }

    IEnumerator DelayedDespawn(float time)
    {
        yield return new WaitForSeconds(time);
        EZ_Pooling.EZ_PoolManager.Despawn(transform);
    }


}
