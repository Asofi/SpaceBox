using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float Speed;                                         // Текущая скорость 
    public Transform PlayerPivot;                               // Точка, вокруг которой вращается игрок
    private int direction = 1;
    public int Size = 1;
    public float CubeOrbitR;
    public float ChangeOrbitSpeed = 3;

    private Orbit curOrbitObj;
    private Vector3 originSize;
    private Vector3 curSize;
    private float minOrbit;
    public int curOrbitNum;
    public bool IsOnOrbit;
    private bool isGoingRight = false;

    void Start () {

        EventManager.OnGameStart += OnGameStart;
        EventManager.OnGameOver += OnGameOver;
        EventManager.OnLevelUp += OnLevelUp;

        originSize = transform.localScale;
        curSize = originSize;

        minOrbit = SuperManager.Instance.GameManager.MinOrbitRadius;
        CubeOrbitR = minOrbit;
        curOrbitNum = 0;
        curOrbitObj = SuperManager.Instance.GameManager.Orbits[curOrbitNum];
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, CubeOrbitR);

    }
	
	void Update () {

        Vector3 newPos = new Vector3(0, (Speed - curOrbitNum * 10) * direction * Time.deltaTime, 0);
        PlayerPivot.Rotate(newPos);
        transform.localPosition = new Vector3(0, 0, CubeOrbitR);

    }

    public void ChangeDirection(bool right)
    {
        if (isGoingRight == right)
            return;
        direction = -direction;
        transform.Rotate(Vector3.forward * 180);
        isGoingRight = !isGoingRight;
    }

    void UdpateCurOrbit(int dir)
    {
        curOrbitObj.isContainsPlayer = false;
        curOrbitNum += dir;
        curOrbitObj = SuperManager.Instance.GameManager.Orbits[curOrbitNum];
        curOrbitObj.isContainsPlayer = true;
    }

    IEnumerator move;
    IEnumerator Move(Orbit targetOrbit)
    {
        IsOnOrbit = false;
        float startPos = CubeOrbitR;
        float t = 0;
        while (t <= 1)
        {
            float newPos = Mathf.Lerp(startPos, targetOrbit.CurRadius, t);
            CubeOrbitR = newPos;
            t += ChangeOrbitSpeed * Time.deltaTime;
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, CubeOrbitR);
            yield return null;
        }
        CubeOrbitR = targetOrbit.CurRadius;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, CubeOrbitR);
        IsOnOrbit = true;
    }

    IEnumerator resize;
    IEnumerator Resize(float targetSize, float time)
    {
        Vector3 target = originSize * targetSize;

        float t = 0;
        while(t < 1)
        {
            Vector3 newSize = Vector3.Lerp(curSize, target, t);
            transform.localScale = newSize;
            t += 1 / time * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.localScale = target;
        curSize = originSize * Size;
    }

    public void StartMoving(int dir)
    {
        if (GameStateManager.GameState != GameStateManager.GameStates.InGame)
            return;

        if(dir == 1)
        {
            if (curOrbitNum == SuperManager.Instance.GameManager.Orbits.Count-1)
                return;
        }
        else
        {
            if (curOrbitNum == 0)
                return;
        }

        UdpateCurOrbit(dir);
        if(move != null)
        {
            StopCoroutine(move);
        }
            move = Move(SuperManager.Instance.GameManager.Orbits[curOrbitNum]);
        StartCoroutine(move);

    }

    private void OnCollisionEnter(Collision collision)
    {
        var tag = collision.gameObject.tag;

        switch (tag)
        {
            case "Planet":
                var planet = collision.transform.GetComponent<Planet>();

                if (planet.Size <= Size)
                {
                    collision.transform.parent.GetComponent<Orbit>().isContainsPlanet = false;
                    collision.gameObject.SetActive(false);
                    Size++;
                    if (resize != null)
                        StopCoroutine(resize);
                    resize = Resize(Size, 0.5f);
                    StartCoroutine(resize);
                    //transform.localScale = originSize * Size;
                    EventManager.AddScore();
                }
                else
                {
                    EventManager.GameOver();
                }
                break;
            case "Asteroid":
                print("ASTEROID");
                if (Size == 6)
                    return;
                StopAllCoroutines();
                EventManager.GameOver();
                break;
            case "Crystall":
                Crystall crystall = collision.gameObject.GetComponent<Crystall>();
                collision.gameObject.SetActive(false);
                crystall.ParentOrbit.isContainsCrystall = false;
                EventManager.AddCrystall();
                break;
        }
        
    }

    void OnLevelUp()
    {
        StopAllCoroutines();
        Size = 1;
        if (resize != null)
            StopCoroutine(resize);
        resize = Resize(Size, 3f);
        StartCoroutine(resize);

        CubeOrbitR = minOrbit;
        curOrbitNum = 0;
        curOrbitObj = SuperManager.Instance.GameManager.Orbits[curOrbitNum];
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, CubeOrbitR);
    }

    void OnGameStart()
    {
        gameObject.SetActive(true);
        CubeOrbitR = minOrbit;
        curOrbitNum = 0;
        curOrbitObj = SuperManager.Instance.GameManager.Orbits[curOrbitNum];
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, CubeOrbitR);
    }

    void OnGameOver()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
        //Set origin size
        Size = 1;
        transform.localScale = originSize;
        curSize = originSize;

    }
}
