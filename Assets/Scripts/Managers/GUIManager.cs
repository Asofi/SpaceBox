using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {

    public CanvasGroup StartScreen;

    public Text CurrentScoreText;
    public Text CrystallsText;



	// Use this for initialization
	void Start () {
        EventManager.OnGameStart += OnGameStart;
        EventManager.OnGameOver += OnGameOver;
        EventManager.OnAddCrystall += OnAddCrystall;
        CrystallsText.text = "Crystalls: " + SuperManager.Instance.IAPManager.Crystalls;
    }


    private void showCanvasGroup(params CanvasGroup[] group)
    {
        foreach (CanvasGroup CG in group)
        {
            CG.alpha = 1;
            CG.interactable = true;
            CG.blocksRaycasts = true;
        }
    }

    private void hideCanvasGroup(params CanvasGroup[] group)
    {
        foreach (CanvasGroup CG in group)
        {
            CG.alpha = 0;
            CG.interactable = false;
            CG.blocksRaycasts = false;
        }
    }

    public void OnAddCrystall()
    {
        CrystallsText.text = "Crystalls: " + SuperManager.Instance.IAPManager.Crystalls;
    }

    /// <summary>
    /// Вызывается в ScoreManager
    /// </summary>
    public void AddScore()
    {
        CurrentScoreText.text = ScoreManager.currentScores.ToString();
    }

    void OnGameStart()
    {
        GameStateManager.GameState = GameStateManager.GameStates.InGame;
        CurrentScoreText.text = ScoreManager.currentScores.ToString();
    }

    void OnGameOver()
    {
        GameStateManager.GameState = GameStateManager.GameStates.OnGameOver;
        showCanvasGroup(StartScreen);
    }

    #region Buttons

    public void StartButton()
    {
        hideCanvasGroup(StartScreen);
        EventManager.GameStart();
    }

    #endregion
}
