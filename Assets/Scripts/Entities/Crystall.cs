using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crystall : MonoBehaviour {

    public Orbit ParentOrbit;
    public Transform Graphics;
    public Image CircleIndicator;

    private float speed = 20;
    private int direction = 1;

    private void Awake()
    {
        ParentOrbit = transform.parent.parent.GetComponent<Orbit>();
        //Graphics = transform.FindChild("GameObject").FindChild("Graphics");
        
    }

    private void Update()
    {
        Vector3 newPos = new Vector3(0, -ParentOrbit.PlanetSpeed * ParentOrbit.PlanetDirection * Time.deltaTime, 0);
        transform.Rotate(newPos);
        Graphics.localEulerAngles = new Vector3(0, Graphics.localEulerAngles.y + speed * Time.deltaTime, 0); 
        //graphics.Rotate(graphics.up, Space.Self);
    }

    public void StartDespawnTimer()
    {
        if(gameObject.activeInHierarchy && ParentOrbit != null)
            StartCoroutine(DelayedDespawn(5 + ParentOrbit.OrbitNum * 1.5f));
    }

    private void OnEnable()
    {
        EventManager.OnTimerStart += StartDespawnTimer;
    }

    private void OnDisable()
    {
        EventManager.OnTimerStart -= StartDespawnTimer;
    }

    IEnumerator DelayedDespawn(float time)
    {
        StartCoroutine(CircleIndication(time));
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
        SuperManager.Instance.GameManager.CurCrystallsCount--;
        ParentOrbit.isContainsCrystall = false;
    }

    IEnumerator CircleIndication(float time)
    {
        float t = time;
        float normalized = 1;
        while(t > 0)
        {
            t -= Time.deltaTime;
            normalized = t / time;
            CircleIndicator.fillAmount = normalized;
            yield return new WaitForEndOfFrame();
        }
    }
}
