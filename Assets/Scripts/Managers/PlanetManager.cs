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
    public float DistanceBetweenOrbits;
    public List<GameObject> Planets;
    public List<Orbit> Orbits;
    public float[] Radiuses;

    public bool isRemoving = false;


	// Use this for initialization
	void Awake () {
        MinOrbitRadius = Sun.localScale.x/2 + DistanceBetweenOrbits;
        Orbits.Add(StartOrbit);
	}

    void Start()
    {
        AddOrbits();
    }
	

    void AddOrbits()
    {
        StartOrbit.DistanceToNext = Radiuses[0] - StartOrbit.Radius;

        for(int i = 0; i< OrbitsCount; i ++)
        {
            float radius;
            //radius = MinOrbitRadius + i * DistanceBetweenOrbits;
            radius = Radiuses[i];

            Transform orbit = Instantiate(OrbitPrefab);
            Orbit orbitScript = orbit.GetComponent<Orbit>();
            Orbits.Add(orbitScript);
            orbitScript.Radius = radius;
            orbitScript.OrbitNum = i + 1;

            if(i == 0)
            {
                orbitScript.DistanceToPrev = orbitScript.DistanceToPrev = radius - StartOrbit.Radius;
                orbitScript.DistanceToNext = Radiuses[i + 1] - radius;
            }
            else if (i == OrbitsCount -1)
            {
                orbitScript.DistanceToPrev = radius - Radiuses[i - 1];
                orbitScript.DistanceToNext = 0;
            }
            else
            {
                orbitScript.DistanceToPrev = radius - Radiuses[i - 1];
                orbitScript.DistanceToNext = Radiuses[i + 1] - radius;
            }

            orbitScript.Planet = Instantiate(SuperManager.Instance.PlanetManager.GetPlanet(), orbit);
            orbitScript.DrawOrbit();
            MaxRadius = radius;


        }
    }

    public void RemoveOrbit(int orbitNum)
    {
        StopAllCoroutines();

        if (orbitNum != 0 && orbitNum != Orbits.Count-1)
        {
            print("Change");
            float buffNext = Orbits[orbitNum - 1].DistanceToNext;

            //Orbits[orbitNum - 1].DistanceToNext = Orbits[orbitNum + 1].DistanceToPrev;
            Orbits[orbitNum + 1].DistanceToPrev = buffNext;
        }
        else if (orbitNum == 0)
        {
            //Orbits[orbitNum - 1].DistanceToNext = Orbits[orbitNum + 1].DistanceToPrev;
        } 
        else if (orbitNum == Orbits.Count - 1)
        {
            //Orbits[orbitNum + 1].DistanceToPrev = Orbits[orbitNum - 1].DistanceToNext;
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
