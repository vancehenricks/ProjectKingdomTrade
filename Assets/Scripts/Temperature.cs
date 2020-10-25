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

    public static float temperature { get; private set; }

    public static float minTemp;
    public static float maxTemp;

    private void Awake()
    {
        Tick.tickUpdate += TickUpdate;
    }

    private void Start()
    {
        temperature = 30f;
        minTemp = 29f;
        maxTemp = 32f;
    }


    // Update is called once per frame
    private void TickUpdate()
    {
        //Debug.Log("temp:" + temperature);

        if (NightDay.isNight() && temperature >= minTemp)
        {
            temperature = temperature - 1;
        }
        else if (!NightDay.isNight() && temperature <= maxTemp)
        {
            temperature = temperature + 1;
        }
    }
}
