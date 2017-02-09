using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {

    public float Speed;
    private int direction;

    private void Start()
    {
        Speed = 5;
        direction = Random.value < 0.5 ? 1 : -1;
    }

    // Update is called once per frame
    void Update () {
        transform.Rotate(Vector3.up, Speed * direction * Time.deltaTime);
    }
}
