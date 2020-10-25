/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimateControl : MonoBehaviour
{

    /*
		temperature = 30f;
		minTemp = 29f;
		maxTemp = 32f;
	*/
    public float springMinTemp;
    public float springMaxTemp;
    public static bool isSpring { get; private set; }

    public float autumMinTemp;
    public float autumnMaxTemp;
    public static bool isAutumn { get; private set; }

    public float winterMinTemp;
    public float winterMaxTemp;
    public static bool isWinter { get; private set; }

    public float summerMinTemp;
    public float summerMaxTemp;
    public static bool isSummer { get; private set; }

    private void Update()
    {

        if (Tick.month >= 8 && Tick.month <= 10 && !isAutumn)
        {
            Temperature.minTemp = autumMinTemp;
            Temperature.maxTemp = autumnMaxTemp;
            isAutumn = true;
            isWinter = false;
            isSpring = false;
            isSummer = false;
        }

        if (Tick.month >= 11 && Tick.month <= 12 && !isWinter)
        {
            Temperature.minTemp = winterMinTemp;
            Temperature.maxTemp = winterMaxTemp;
            isAutumn = false;
            isWinter = true;
            isSpring = false;
            isSummer = false;
        }

        if (Tick.month >= 1 && Tick.month <= 4 && !isSpring)
        {
            Temperature.minTemp = springMinTemp;
            Temperature.maxTemp = springMaxTemp;
            isAutumn = false;
            isWinter = false;
            isSpring = true;
            isSummer = false;
        }

        if (Tick.month >= 5 && Tick.month < 8 && !isSummer)
        {
            Temperature.minTemp = summerMinTemp;
            Temperature.maxTemp = summerMaxTemp;
            isAutumn = false;
            isWinter = false;
            isSpring = false;
            isSummer = true;
        }
    }
}
