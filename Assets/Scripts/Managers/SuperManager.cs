using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperManager : MonoBehaviour {

    private static SuperManager instance;
    public static SuperManager Instance { get { return instance; } }

    private void Awake()
    {
        instance = this;
    }

    public GameManager GameManager;
    public GUIManager GUIManager;
    public ScoreManager ScoreManager;
    public IAPManager IAPManager;
    public DifficultyManager DifficultyManager;

    public Player Player;
}
