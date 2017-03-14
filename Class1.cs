
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
    RightDown = 10,
}

public class SwipeManager : MonoBehaviour
{

    private static SwipeManager instance;
    public static SwipeManager Instance { get { return instance; } }

    public SwipeDirection HorDirection { set; get; }
    public SwipeDirection VertDirection { set; get; }

    private Vector3 touchPosition;
    public float SwipeResistanceX = 50;
    public float SwipeResistanceY = 100;

    // Use this for initialization
    void Start()
    {
        instance = this;
    }

    void Update()
    {

        HorDirection = SwipeDirection.None;
        VertDirection = SwipeDirection.None;

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
            }


            if (Mathf.Abs(deltaSwipe.y) > SwipeResistanceY)
            {
                VertDirection |= (deltaSwipe.y > 0) ? SwipeDirection.Down : SwipeDirection.Up;
            }
        }

    }

}