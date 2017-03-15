public static class EventManager
{

    public delegate void GameEvent();

    public static event GameEvent OnGameStart, OnGameOver,
                                    OnAddScore, OnMute,
                                    OnAddCrystall, OnLevelUp,
                                    OnMoveOrbit, OnTimerStart;

    public static void GameOver()
    {
        if (OnGameOver != null) OnGameOver();
    }

    public static void GameStart()
    {
        if (OnGameStart != null) OnGameStart();
    }


    public static void AddScore()
    {
        if (OnAddScore != null) OnAddScore();
    }

    public static void AddCrystall()
    {
        if (OnAddCrystall != null) OnAddCrystall();
    }

    public static void Mute()
    {
        if (OnMute != null) OnMute();
    }

    public static void LevelUp()
    {
        if (OnLevelUp != null) OnLevelUp();
    }

    public static void UnlockFish()
    {
        if (OnMoveOrbit != null) OnMoveOrbit();
    }

    public static void TimerStart()
    {
        if (OnTimerStart != null) OnTimerStart();
    }

}
