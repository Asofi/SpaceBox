using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameStateManager {

    public enum GameStates
    {
        OnStartScreen,
        InGame,
        OnGameOver,
        OnPauseScreen
    }

    public static GameStates GameState = GameStates.OnStartScreen;

}
