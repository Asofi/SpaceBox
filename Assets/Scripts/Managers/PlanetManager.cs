using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour {

    public Transform OrbitPrefab;
    public Orbit StartOrbit;
    public Transform Sun;
    public float MinOrbitRadius;
    public int OrbitsCount;
    public float MaxRadius;
    public float DistanceToFirstOrbit;
    public List<GameObject> Planets;
    public List<Orbit> Orbits;
    public float[] Radiuses;

    public bool isRemoving = false;


	// Use this for initialization
	void Awake () {
        MinOrbitRadius = Sun.localScale.x/2 + DistanceToFirstOrbit;
        Orbits.Add(StartOrbit);
	}

    void Start()
    {
        AddOrbits();
    }
	

    void AddOrbits()
    {
        StartOrbit.DistanceToNext = Radiuses[0] - StartOrbit.Radius;
        Orbit prevOrbit = StartOrbit;

        for(int i = 0; i< OrbitsCount; i ++)
        {

            float radius;
            Transform orbit = Instantiate(OrbitPrefab);
            Orbit orbitScript = orbit.GetComponent<Orbit>();
            orbitScript.Planet = Instantiate(SuperManager.Instance.PlanetManager.GetPlanet(), orbit);
            if (i == 0)
                radius = prevOrbit.Radius + orbitScript.Planet.GetComponent<Planet>().Size * 2;
            else
                radius = prevOrbit.Radius + prevOrbit.Planet.GetComponent<Planet>().Size + 1.5f * orbitScript.Planet.GetComponent<Planet>().Size; 


            Orbits.Add(orbitScript);
            orbitScript.Radius = radius;
            Radiuses[i] = radius;
            orbitScript.OrbitNum = i + 1;

            if (prevOrbit == orbitScript)
            print(radius - prevOrbit.Radius);
                orbitScript.DistanceToPrev = radius - prevOrbit.Radius;
                prevOrbit.DistanceToNext = orbitScript.DistanceToPrev;

            orbitScript.DrawOrbit();
            MaxRadius = radius;


            prevOrbit = orbitScript;
        }
    }

    public void RemoveOrbit(int orbitNum)
    {
        StopAllCoroutines();

        if (orbitNum != 0 && orbitNum != Orbits.Count-1)
        {
            float buffNext = Orbits[orbitNum - 1].DistanceToNext;
            Orbits[orbitNum + 1].DistanceToPrev = buffNext;
        }

        isRemoving = true;
        var orbit = Orbits[orbitNum];
        var removedOrbitsDistToNext = orbit.DistanceToNext;
        Orbits.RemoveAt(orbitNum);
        orbit.StopAllCoroutines();
        Destroy(orbit.gameObject);
        if (SuperManager.Instance.Player.curOrbitNum >= orbitNum)
            SuperManager.Instance.Player.curOrbitNum--;


        for (int i = orbitNum; i < Orbits.Count; i++)
        {
                StartCoroutine(Orbits[i].StartMoveOrbits(Orbits[i].Radius - removedOrbitsDistToNext));
        }
    }

    public GameObject GetPlanet()
    {
        if (Planets.Count > 0)
        {
            var num = Random.Range(0, Planets.Count);
            var buff = Planets[num];
            Planets.RemoveAt(num);
            return buff;
        }
        return null;
    }
}
