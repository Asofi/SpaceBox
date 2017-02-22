using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    enum PlayerSectors
    {
        Up,
        Down,
    }

    PlayerSectors curPlayerSector;

    public float Speed;                                         // Текущая скорость 
    public Transform PlayerPivot;                               // Точка, вокруг которой вращается игрок
    private int direction = 1;
    public int Size = 1;
    public float CubeOrbitR;
    public float ChangeOrbitSpeed = 3;

    private Orbit curOrbitObj;
    private float minOrbit;
    public int curOrbitNum;
    public bool IsOnOrbit;

    void Start () {

        EventManager.OnGameStart += OnGameStart;
        EventManager.OnGameOver += OnGameOver;
        EventManager.OnLevelUp += OnLevelUp;

        minOrbit = SuperManager.Instance.PlanetManager.MinOrbitRadius;
        CubeOrbitR = minOrbit;
        curOrbitNum = 0;
        curOrbitObj = SuperManager.Instance.PlanetManager.Orbits[curOrbitNum];
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, CubeOrbitR);

    }
	
	void Update () {

        Vector3 newPos = new Vector3(0, Speed * direction * Time.deltaTime, 0);
        PlayerPivot.Rotate(newPos);
        transform.localPosition = new Vector3(0, 0, CubeOrbitR);
        CheckSector();

        switch (curPlayerSector)
        {
            case PlayerSectors.Down:
                if (SwipeManager.Instance.VertDirection == SwipeDirection.Up /*|| SwipeManager.Instance.Direction == SwipeDirection.LeftUp || SwipeManager.Instance.Direction == SwipeDirection.RightUp*/)
                {
                    MoveBack();
                }

                if (SwipeManager.Instance.VertDirection == SwipeDirection.Down /*|| SwipeManager.Instance.Direction == SwipeDirection.LeftDown || SwipeManager.Instance.Direction == SwipeDirection.RightDown*/)
                {
                    MoveForvard();
                }

                if (SwipeManager.Instance.HorDirection == SwipeDirection.Left)
                {
                    direction = 1;
                }

                if (SwipeManager.Instance.HorDirection == SwipeDirection.Right)
                {
                    direction = -1;
                }
                break;

            case PlayerSectors.Up:

                if (SwipeManager.Instance.VertDirection == SwipeDirection.Up /*|| SwipeManager.Instance.Direction == SwipeDirection.LeftUp || SwipeManager.Instance.Direction == SwipeDirection.RightUp*/)
                {
                    MoveForvard();
                }

                if (SwipeManager.Instance.VertDirection == SwipeDirection.Down /*|| SwipeManager.Instance.Direction == SwipeDirection.LeftDown || SwipeManager.Instance.Direction == SwipeDirection.RightDown*/)
                {
                    MoveBack();
                }

                if (SwipeManager.Instance.HorDirection == SwipeDirection.Left)
                {
                    direction = -1;
                }

                if (SwipeManager.Instance.HorDirection == SwipeDirection.Right)
                {
                    direction = 1;
                }
                break;
        }

    }

    void UdpateCurOrbit(int dir)
    {
        curOrbitObj.isContainsPlayer = false;
        curOrbitNum += dir;
        curOrbitObj = SuperManager.Instance.PlanetManager.Orbits[curOrbitNum];
        curOrbitObj.isContainsPlayer = true;
    }

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

    void MoveBack()
    {
        if (curOrbitNum == 0)
            return;

        UdpateCurOrbit(-1);

        StartCoroutine(Move(SuperManager.Instance.PlanetManager.Orbits[curOrbitNum]));

    } 

    void MoveForvard()
    {
        if (curOrbitNum == SuperManager.Instance.PlanetManager.Orbits.Count-1)
            return;

        UdpateCurOrbit(1);

        StartCoroutine(Move(SuperManager.Instance.PlanetManager.Orbits[curOrbitNum]));

    }

    void CheckSector()
    {
        if (PlayerPivot.eulerAngles.y > 0 && PlayerPivot.eulerAngles.y < 180)
        {
            curPlayerSector = PlayerSectors.Up;
        }
        else if ((PlayerPivot.eulerAngles.y > 180 && PlayerPivot.eulerAngles.y < 360))
        {
            curPlayerSector = PlayerSectors.Down;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        var planet = collision.transform.GetComponent<Planet>();

        if (planet.Size <= Size)
        {
            collision.transform.parent.GetComponent<Orbit>().isContainsPlanet = false;
            collision.gameObject.SetActive(false);
            transform.localScale = transform.localScale + Vector3.one * 1.5f;
            Size++;
            EventManager.AddScore();
        }
        else
        {
            gameObject.SetActive(false);
            EventManager.GameOver();
        }
    }

    void OnLevelUp()
    {
        Size = 1;
        transform.localScale = Vector3.one;
    }

    void OnGameStart()
    {
        gameObject.SetActive(true);
        CubeOrbitR = minOrbit;
        curOrbitNum = 0;
        curOrbitObj = SuperManager.Instance.PlanetManager.Orbits[curOrbitNum];
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, CubeOrbitR);
    }

    void OnGameOver()
    {
        //Set origin size
        Size = 1;
        transform.localScale = Vector3.one;

        StopAllCoroutines();
    }
}
