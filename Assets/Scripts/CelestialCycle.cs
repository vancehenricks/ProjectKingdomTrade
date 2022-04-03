/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, January 2019
 */
using UnityEngine;

public class CelestialCycle : MonoBehaviour
{

    public Transform sun;
    public Transform moon;

    public Transform pointA;
    public Transform pointB;

    private void Update()
    {
        if (Tick.init.speed <= 0)
        {
            return;
        }

        if (NightDay.init.isNight())
        {
            sun.position = Vector3.Lerp(sun.position, pointB.position, (Tick.init.speed * 0.2f) * Time.deltaTime);
            moon.position = Vector3.Lerp(moon.position, pointA.position, (Tick.init.speed * 0.8f) * Time.deltaTime);
        }
        else if (!NightDay.init.isNight())
        {
            sun.position = Vector3.Lerp(sun.position, pointA.position, (Tick.init.speed * 0.8f) * Time.deltaTime);
            moon.position = Vector3.Lerp(moon.position, pointB.position, (Tick.init.speed * 0.2f) * Time.deltaTime);
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
