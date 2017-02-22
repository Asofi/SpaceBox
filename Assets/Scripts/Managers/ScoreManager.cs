using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {

    private const string BEST_SCORES = "best_scores";

    public static int currentScores;
    private int best;


    void Start()
    {
        Application.targetFrameRate = 60;

        EventManager.OnGameStart += OnGameStart;
        EventManager.OnGameOver += OnGameOver;
        EventManager.OnAddScore += OnAddScore;

        best = PlayerPrefs.GetInt(BEST_SCORES);
        currentScores = 0;

    }

    public int GetCurrentScores()
    {
        return currentScores;
    }

    public int GetBestScores()
    {
        return best;
    }

    private void OnAddScore()
    {
        currentScores++;
        SuperManager.Instance.GUIManager.AddScore();

    }

    private void OnGameStart()
    {
        currentScores = 0;
        enabled = true;
    }

    private void OnGameOver()
    {
        if (currentScores > best)
        {
            best = currentScores;
            PlayerPrefs.SetInt(BEST_SCORES, best);
            //SuperManager.Instance.ServicesManager.SendScoreToLeaderboard(best);
            //SuperManager.Instance.GUIManager.NewRecordLayout.SetActive(true);
        }
        enabled = false;
    }
}
