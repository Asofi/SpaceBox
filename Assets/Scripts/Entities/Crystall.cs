using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystall : MonoBehaviour {

    public Orbit ParentOrbit;

    private void Start()
    {
        ParentOrbit = transform.parent.parent.GetComponent<Orbit>();
    }
}
