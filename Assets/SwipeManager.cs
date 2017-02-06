using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SwipeDurection
{
    None = 0,
    Left = 1,
    Right = 2,
    Up = 4,
    Down = 8,
}

public class SwipeManager : MonoBehaviour {

    private static SwipeManager instance;
    public static SwipeManager Instance { get { return instance; } }

    public SwipeDurection Direction { set; get; }

    private Vector3 touchPosition;
    public float SwipeResistanceX = 50;
    public float SwipeResistanceY = 100;

	// Use this for initialization
	void Start () {
        instance = this;
	}
	
	void Update () {

        Direction = SwipeDurection.None;

        if (Input.GetMouseButtonDown(0))
        {
            touchPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector2 deltaSwipe = touchPosition - Input.mousePosition;

            if(Mathf.Abs(deltaSwipe.x) > SwipeResistanceX)
            {
                Direction |= (deltaSwipe.x > 0) ? SwipeDurection.Left : SwipeDurection.Right;
            }


            if (Mathf.Abs(deltaSwipe.y) > SwipeResistanceY)
            {
                Direction |= (deltaSwipe.y > 0) ? SwipeDurection.Down : SwipeDurection.Up;
            }
        }
	}
}
