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
    private int size = 1;
    public float CubeOrbitR;
    private float distanceBetweenOrbits;
    public float distanceToNext;
    public float distanceToPrev;

    private Orbit curOrbitObj;
    private float minOrbit;
    public int curOrbitNum;

    // Use this for initialization
    void Start () {
        distanceBetweenOrbits = SuperManager.Instance.PlanetManager.DistanceBetweenOrbits;
        minOrbit = SuperManager.Instance.PlanetManager.MinOrbitRadius;
        CubeOrbitR = minOrbit;
        curOrbitNum = 0;
        curOrbitObj = SuperManager.Instance.PlanetManager.Orbits[curOrbitNum];
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, CubeOrbitR);

        distanceToNext = SuperManager.Instance.PlanetManager.StartOrbit.DistanceToNext;
        distanceToPrev = SuperManager.Instance.PlanetManager.StartOrbit.DistanceToPrev;

    }
	
	// Update is called once per frame
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
        print("Update");
        curOrbitObj.isContainsPlayer = false;
        curOrbitNum += dir;
        curOrbitObj = SuperManager.Instance.PlanetManager.Orbits[curOrbitNum];
        curOrbitObj.isContainsPlayer = true;
        distanceToNext = curOrbitObj.DistanceToNext;
        distanceToPrev = curOrbitObj.DistanceToPrev;
    }

    void MoveBack()
    {
        if (curOrbitNum == 0)
            return;

        float newOrbit = CubeOrbitR;
        newOrbit = CubeOrbitR - distanceToPrev;
        UdpateCurOrbit(-1);

        CubeOrbitR = Mathf.Clamp(newOrbit, minOrbit, SuperManager.Instance.PlanetManager.MaxRadius);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, CubeOrbitR);

    } 

    void MoveForvard()
    {
        if (curOrbitNum == SuperManager.Instance.PlanetManager.Orbits.Count-1)
            return;

        float newOrbit = CubeOrbitR;
        newOrbit = CubeOrbitR + distanceToNext;
        UdpateCurOrbit(1);

        CubeOrbitR = Mathf.Clamp(newOrbit, minOrbit, SuperManager.Instance.PlanetManager.MaxRadius);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, CubeOrbitR);

    }

    void CheckSector()
    {
        if (PlayerPivot.eulerAngles.y > 90 && PlayerPivot.eulerAngles.y < 270)
        {
            curPlayerSector = PlayerSectors.Down;
        }
        else if ((PlayerPivot.eulerAngles.y > 270 && PlayerPivot.eulerAngles.y < 360) || (PlayerPivot.eulerAngles.y > 0 && PlayerPivot.eulerAngles.y < 90))
        {
            curPlayerSector = PlayerSectors.Up;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        var planet = collision.transform.parent.GetComponent<Orbit>();

        if (planet.PlanetSize.x <= transform.localScale.x)
        {
            collision.transform.parent.GetComponent<Orbit>().isContainsPlanet = false;
            Destroy(collision.gameObject);
            transform.localScale = transform.localScale + Vector3.one;
        }
        else
            Destroy(gameObject);
    }
}
