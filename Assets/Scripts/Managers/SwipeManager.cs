using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


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
    public PlayerSectors curPlayerSector { set; get; }

    private Vector3 touchPosition;
    //public float SwipeResistanceX = 50;
    //public float SwipeResistanceY = 100;

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
        if (SuperManager.Instance.GameManager.IsLevelUping)
            return;
        Swipe();

    }

    //void CheckSector()
    //{
    //    if (PlayerPivot.eulerAngles.y > 0 && PlayerPivot.eulerAngles.y < 180)
    //    {
    //        curPlayerSector = PlayerSectors.Up;
    //    }
    //    else if ((PlayerPivot.eulerAngles.y > 180 && PlayerPivot.eulerAngles.y < 360))
    //    {
    //        curPlayerSector = PlayerSectors.Down;
    //    }
    //}

    //public bool IsSwiping(SwipeDirection dir)
    //{
    //    return dir == Direction;
    //}



    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;

    public void Swipe()
    {
        if ((GameStateManager.IsMobile ? EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) : EventSystem.current.IsPointerOverGameObject()))
            return;

        if (Input.GetMouseButtonDown(0))
        {
            //save began touch 2d point
            firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        if (Input.GetMouseButtonUp(0))
        {
            //save ended touch 2d point
            secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            //create vector from the two points
            currentSwipe = new Vector2(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

            //normalize the 2d vector
            currentSwipe.Normalize();

            //swipe upwards
            if (currentSwipe.y > 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
            {
                SuperManager.Instance.Player.StartMoving(1);
            }
            else
            //swipe down
            if (currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
            {
                SuperManager.Instance.Player.StartMoving(-1);
            }
            else
            //swipe left
            if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
            {
                SuperManager.Instance.Player.ChangeDirection(true);
            }
            else
            //swipe right
            if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
            {
                SuperManager.Instance.Player.ChangeDirection(false);
            }
            else
                //tap
                SuperManager.Instance.Player.ChangeDirection();
        }
    }

}
