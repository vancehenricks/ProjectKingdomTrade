/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temperature : MonoBehaviour
{
    private static Temperature _init;

    public static Temperature init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public float temperature { get; private set; }

    public float minTemp;
    public float maxTemp;

    private void Awake()
    {
        init = this;
    }

    private void OnDestroy()
    {
        Tick.init.tickUpdate -= TickUpdate;
    }

    private void Start()
    {
        Tick.init.tickUpdate += TickUpdate;
        temperature = 30f;
        minTemp = 29f;
        maxTemp = 32f;
    }


    // Update is called once per frame
    private void TickUpdate()
    {
        //Debug.Log("temp:" + temperature);

        if (NightDay.init.isNight() && temperature >= minTemp)
        {
            temperature = temperature - 1;
        }
        else if (!NightDay.init.isNight() && temperature <= maxTemp)
        {
            temperature = temperature + 1;
        }
    }
}
