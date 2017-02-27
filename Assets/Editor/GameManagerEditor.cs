using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor {


    void OnSceneGUI()
    {
        GameManager gm = (GameManager)target;
        Handles.color = Color.red;
        Handles.DrawWireArc(Vector3.zero, Vector3.up, Vector3.forward, 360, gm.AsteroidSpawnRadius);
    }
}
