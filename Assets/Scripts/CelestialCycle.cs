/* Copyright (C) 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, January 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CelestialCycle : MonoBehaviour {

	public Transform sun;
	public Transform moon;

	public Transform pointA;
	public Transform pointB;

	private void Update ()
    {
		
		if(Tick.speed <= 0)
        {
			return;
		}

		if(NightDay.isNight())
        {
			sun.position = Vector3.Lerp(sun.position, pointB.position, (Tick.speed * 0.2f) * Time.deltaTime);
			moon.position = Vector3.Lerp(moon.position, pointA.position, (Tick.speed * 0.8f) * Time.deltaTime);
		} else if (!NightDay.isNight())
        {
			sun.position = Vector3.Lerp(sun.position, pointA.position, (Tick.speed * 0.8f) * Time.deltaTime);
			moon.position = Vector3.Lerp(moon.position, pointB.position, (Tick.speed * 0.2f) * Time.deltaTime);
		}

	}

	public void Initialize()
    {
		if(NightDay.isNight())
        {
			sun.position = pointB.position;
			moon.position = pointA.position;
		} else if (!NightDay.isNight()) {
			sun.position = pointA.position;
			moon.position = pointB.position;
		}
	}
}
