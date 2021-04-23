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
    private static ClimateControl _init;

    public static ClimateControl init
    {
        get { return _init; }
        private set { _init = value; }
    }

    /*
		temperature = 30f;
		minTemp = 29f;
		maxTemp = 32f;
	*/
    public float springMinTemp;
    public float springMaxTemp;
    public bool isSpring { get; private set; }

    public float autumMinTemp;
    public float autumnMaxTemp;
    public bool isAutumn { get; private set; }

    public float winterMinTemp;
    public float winterMaxTemp;
    public bool isWinter { get; private set; }

    public float summerMinTemp;
    public float summerMaxTemp;
    public bool isSummer { get; private set; }

    private void Awake()
    {
        init = this;
    }

    private void FixedUpdate()
    {

        if (Tick.init.month >= 8 && Tick.init.month <= 10 && !isAutumn)
        {
            Temperature.init.minTemp = autumMinTemp;
            Temperature.init.maxTemp = autumnMaxTemp;
            isAutumn = true;
            isWinter = false;
            isSpring = false;
            isSummer = false;
        }
        else if (Tick.init.month >= 11 && Tick.init.month <= 12 && !isWinter)
        {
            Temperature.init.minTemp = winterMinTemp;
            Temperature.init.maxTemp = winterMaxTemp;
            isAutumn = false;
            isWinter = true;
            isSpring = false;
            isSummer = false;
        }
        else if (Tick.init.month >= 1 && Tick.init.month <= 4 && !isSpring)
        {
            Temperature.init.minTemp = springMinTemp;
            Temperature.init.maxTemp = springMaxTemp;
            isAutumn = false;
            isWinter = false;
            isSpring = true;
            isSummer = false;
        }
        else if (Tick.init.month >= 5 && Tick.init.month < 8 && !isSummer)
        {
            Temperature.init.minTemp = summerMinTemp;
            Temperature.init.maxTemp = summerMaxTemp;
            isAutumn = false;
            isWinter = false;
            isSpring = false;
            isSummer = true;
        }
    }
}
