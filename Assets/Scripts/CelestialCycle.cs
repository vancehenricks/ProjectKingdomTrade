/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, January 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

public struct Celestial
{
    public Vector3 sun;
    public Vector3 moon;
    public Vector3 pointB;
    public Vector3 pointA;
    public float TickSpeed;
    public float speedModifier;
    public float deltaTime;
    public bool isNight;
}

public class CelestialCycle : MonoBehaviour
{

    public Transform sun;
    public Transform moon;

    public Transform pointA;
    public Transform pointB;

    private ParallelInstance<Celestial> parallelInstance;
    private Celestial celestial;

    private void Start()
    {
        celestial = new Celestial();
        parallelInstance = new ParallelInstance<Celestial>(Calculate, (Celestial _celestial, Celestial orginal) => { celestial = _celestial; });
        StartCoroutine(Cycle());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    //Seperate thread+
    private void Calculate(System.Action<Celestial,Celestial> _result, Celestial _celestial)
    {
        Celestial newCelestial = new Celestial();

        if (_celestial.isNight)
        {
            newCelestial.sun = Vector3.Lerp(_celestial.sun, _celestial.pointB, (_celestial.TickSpeed * 0.2f) * _celestial.deltaTime);
            newCelestial.moon = Vector3.Lerp(_celestial.moon, _celestial.pointA, (_celestial.TickSpeed * 0.8f) * _celestial.deltaTime);
        }
        else
        {
            newCelestial.sun = Vector3.Lerp(_celestial.sun, _celestial.pointA, (_celestial.TickSpeed * 0.8f) * _celestial.deltaTime);
            newCelestial.moon = Vector3.Lerp(_celestial.moon, _celestial.pointB, (_celestial.TickSpeed * 0.2f) * _celestial.deltaTime);
        }

        _result(newCelestial,_celestial);
    }
    //Seperate thread-

    private IEnumerator Cycle()
    {
        while (true)
        {
            if (Tick.init.speed <= 0)
            {
                yield return null;
            }

            celestial.sun = sun.position;
            celestial.moon = moon.position;
            celestial.pointA = pointA.position;
            celestial.pointB = pointB.position;
            celestial.TickSpeed = Tick.init.speed;
            celestial.deltaTime = Time.deltaTime;
            celestial.isNight = NightDay.init.isNight();

            parallelInstance.Set(celestial);
            Task task = new Task(parallelInstance.Calculate);
            task.Start();

            yield return null;

            //task.Wait();
            sun.position = celestial.sun;
            moon.position = celestial.moon;
        }
    }

    public void Initialize()
    {
        if (NightDay.init.isNight())
        {
            sun.position = pointB.position;
            moon.position = pointA.position;
        }
        else if (!NightDay.init.isNight())
        {
            sun.position = pointA.position;
            moon.position = pointB.position;
        }
    }
}
