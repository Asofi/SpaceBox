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

    public Transform OffscreenIndicator;
    private Vector3 PositionOnScreen;
    private bool isOnScreen = false;


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
        OffscreenIndicator.gameObject.SetActive(true);
        OffscreenIndicator.rotation = Quaternion.Euler(-90,-90,0);

        Speed = SuperManager.Instance.DifficultyManager.GetAsteroidSpeed();
        mRigid.AddForce(transform.forward * 100 * Speed);

    }

    void OnDespawned()
    {
        StopAllCoroutines();
    }


    private void Update()
    {
        if (!IsOnScreen())
            DrawOffScreenIndicator();
        else
            OffscreenIndicator.gameObject.SetActive(false);
    }

    bool IsOnScreen()
    {
        while (true)
        {
            PositionOnScreen = Camera.main.WorldToScreenPoint(transform.position);
            if (PositionOnScreen.x > 0 && PositionOnScreen.y > 0 && PositionOnScreen.x < Screen.width && PositionOnScreen.y < Screen.height && !deadPlaying)
                return true;
            else
                return false;
        }
    }

    void DrawOffScreenIndicator()
    {
        Vector3 indicatorPos;
        indicatorPos.x = Mathf.Clamp(transform.position.x, GameManager.WorldsSpaceScreenBorders.y, GameManager.WorldsSpaceScreenBorders.w);
        indicatorPos.z = Mathf.Clamp(transform.position.z, GameManager.WorldsSpaceScreenBorders.z, GameManager.WorldsSpaceScreenBorders.x);
        indicatorPos.y = 20;
        OffscreenIndicator.position = indicatorPos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        DestroyAsteroid();
    }

    IEnumerator delayedDespawn;
    IEnumerator DelayedDespawn(float time)
    {
        deadPlaying = true;
        isOnScreen = false;
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
        OffscreenIndicator.gameObject.SetActive(false);
        if (!deadPlaying && gameObject.activeSelf)
            StartCoroutine(DelayedDespawn(4));
    }


}
