using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAPManager : MonoBehaviour {

    private const string CRYSTALLS = "crystalls";

    private int crystalls;

    public int Crystalls
    {
        get
        {
            return crystalls;
        }
    }

    // Use this for initialization
    void Start () {
        EventManager.OnGameStart += OnGameStart;
        EventManager.OnGameOver += OnGameOver;
        EventManager.OnAddCrystall += OnAddCrystall;

        crystalls = PlayerPrefs.GetInt(CRYSTALLS);
    }

    private void OnAddCrystall()
    {
        crystalls++;
        PlayerPrefs.SetInt(CRYSTALLS, crystalls);
    }

    private void OnGameStart()
    {
    }

    private void OnGameOver()
    {

    }
}
