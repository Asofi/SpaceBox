﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum SwipeDirection
{
    None = 0,
    Left = 1,
    Right = 2,
    Up = 4,
    Down = 8,
    LeftUp = 5,
    LeftDown = 9,
    RightUp = 6,
    RightDown = 10
}

public enum PlayerSectors
{
    Up,
    Down
}


public class SwipeManager : MonoBehaviour {

    private static SwipeManager instance;
    public static SwipeManager Instance { get { return instance; } }

    public LayerMask TouchMask;
    public Transform PlayerPivot;
    public SwipeDirection Direction { set; get; }
    public SwipeDirection HorDirection { set; get; }
    public SwipeDirection VertDirection { set; get; }
    public PlayerSectors curPlayerSector { set; get; }

    private Vector3 touchPosition;
    public float SwipeResistanceX = 50;
    public float SwipeResistanceY = 100;

	// Use this for initialization
	void Start () {
        instance = this;

#if UNITY_EDITOR
        GameStateManager.IsMobile = false;
#else
        GameStateManager.IsMobile = true;
#endif

    }
	
	void Update () {
        if (SuperManager.Instance.GameManager.isLevelUping)
            return;

        HorDirection = SwipeDirection.None;
        VertDirection = SwipeDirection.None;
        if ((GameStateManager.IsMobile ? !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) : !EventSystem.current.IsPointerOverGameObject()))
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchPosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                Vector2 deltaSwipe = touchPosition - Input.mousePosition;

                if (Mathf.Abs(deltaSwipe.x) > SwipeResistanceX)
                {
                    HorDirection |= (deltaSwipe.x > 0) ? SwipeDirection.Left : SwipeDirection.Right;
                    CheckSector();
                    switch (curPlayerSector)
                    {
                        case PlayerSectors.Down:

                            if (HorDirection == SwipeDirection.Left)
                            {
                                SuperManager.Instance.Player.ChangeDirection(false);
                            }

                            if (HorDirection == SwipeDirection.Right)
                            {
                                SuperManager.Instance.Player.ChangeDirection(true);
                            }
                            break;

                        case PlayerSectors.Up:

                            if (HorDirection == SwipeDirection.Left)
                            {
                                SuperManager.Instance.Player.ChangeDirection(true);
                            }

                            if (HorDirection == SwipeDirection.Right)
                            {
                                SuperManager.Instance.Player.ChangeDirection(false);
                            }
                            break;
                    }
                }
                else
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(touchPosition);

                    if (Physics.Raycast(ray, out hit, TouchMask))
                    {
                        var PlayerPos = SuperManager.Instance.Player.CubeOrbitR;

                        if (Mathf.Pow(hit.point.x, 2) + Mathf.Pow(hit.point.z, 2) <= (PlayerPos * PlayerPos))
                            SuperManager.Instance.Player.StartMoving(-1);
                        else
                            SuperManager.Instance.Player.StartMoving(1);
                    }
                }




                //if (Mathf.Abs(deltaSwipe.y) > SwipeResistanceY)
                //{
                //    VertDirection |= (deltaSwipe.y > 0) ? SwipeDirection.Down : SwipeDirection.Up;
                //}
            }
        }

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

}
