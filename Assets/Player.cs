using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    enum PlayerSectors
    {
        Up,
        Down,
        Left,
        Right
    }

    PlayerSectors curPlayerSector;

    public float Speed;                                         // Текущая скорость 
    public Transform PlayerPivot;                               // Точка, вокруг которой вращается игрок
    private int direction = 1;
    public float orbit = 1;
    public float DistanceBetweenOrbits = 0.5f;

    private float minOrbit = 1;
    private float maxOrbit = 5;

    // Use this for initialization
    void Start () {
        transform.localPosition = new Vector3(0, 0, orbit);	
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 newPos = new Vector3(0, Speed * direction * Time.deltaTime, 0);
        float newOrbit = orbit;
        PlayerPivot.Rotate(newPos);
        CheckSector();

        switch (curPlayerSector)
        {
            case PlayerSectors.Down:
                if (SwipeManager.Instance.Direction == SwipeDurection.Up)
                {
                    newOrbit = orbit - DistanceBetweenOrbits;
                }

                if (SwipeManager.Instance.Direction == SwipeDurection.Down)
                {
                    newOrbit = orbit + DistanceBetweenOrbits;
                }

                if (SwipeManager.Instance.Direction == SwipeDurection.Left)
                {
                    direction = 1;
                }

                if (SwipeManager.Instance.Direction == SwipeDurection.Right)
                {
                    direction = -1;
                }
                break;

            case PlayerSectors.Up:
                if (SwipeManager.Instance.Direction == SwipeDurection.Up)
                {
                    newOrbit = orbit + DistanceBetweenOrbits;
                }

                if (SwipeManager.Instance.Direction == SwipeDurection.Down)
                {
                    newOrbit = orbit - DistanceBetweenOrbits;
                }

                if (SwipeManager.Instance.Direction == SwipeDurection.Left)
                {
                    direction = -1;
                }

                if (SwipeManager.Instance.Direction == SwipeDurection.Right)
                {
                    direction = 1;
                }
                break;

            case PlayerSectors.Left:
                if (SwipeManager.Instance.Direction == SwipeDurection.Up)
                {
                    direction = 1;
                }

                if (SwipeManager.Instance.Direction == SwipeDurection.Down)
                {
                    direction = -1;
                }

                if (SwipeManager.Instance.Direction == SwipeDurection.Left)
                {
                    newOrbit = orbit + DistanceBetweenOrbits;
                }

                if (SwipeManager.Instance.Direction == SwipeDurection.Right)
                {
                    newOrbit = orbit - DistanceBetweenOrbits;
                }
                break;

            case PlayerSectors.Right:
                if (SwipeManager.Instance.Direction == SwipeDurection.Up)
                {
                    direction = -1;
                }

                if (SwipeManager.Instance.Direction == SwipeDurection.Down)
                {
                    direction = 1;
                }

                if (SwipeManager.Instance.Direction == SwipeDurection.Left)
                {
                    newOrbit = orbit - DistanceBetweenOrbits;
                }

                if (SwipeManager.Instance.Direction == SwipeDurection.Right)
                {
                    newOrbit = orbit + DistanceBetweenOrbits;
                }
                break;
        }

        orbit = Mathf.Clamp(newOrbit, minOrbit, maxOrbit);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, orbit);
    }

    void CheckSector()
    {
        if(PlayerPivot.eulerAngles.y > 45 && PlayerPivot.eulerAngles.y < 135)
        {
            curPlayerSector = PlayerSectors.Right;
        }
        else if (PlayerPivot.eulerAngles.y > 135 && PlayerPivot.eulerAngles.y < 225)
        {
            curPlayerSector = PlayerSectors.Down;
        }
        else if ((PlayerPivot.eulerAngles.y > 225 && PlayerPivot.eulerAngles.y < 315))
        {
            curPlayerSector = PlayerSectors.Left;
        }
        else if ((PlayerPivot.eulerAngles.y > 315 && PlayerPivot.eulerAngles.y < 360) || (PlayerPivot.eulerAngles.y > 0 && PlayerPivot.eulerAngles.y < 45))
        {
            curPlayerSector = PlayerSectors.Up;
        }
    }
}
