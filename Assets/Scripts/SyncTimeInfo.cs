/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SyncTimeInfo : MonoBehaviour
{

    public Text temperature;
    public Text time;
    public Text date;
    public Text speed;

    // Update is called once per frame
    private void Awake()
    {
        Tick.init.tickUpdate += TickUpdate;
    }

    private void Update()
    {
        speed.text = "x" + Tick.init.speed;
    }

    private void TickUpdate()
    {
        time.text = "" + Tick.init.seconds;
        temperature.text = "" + Mathf.RoundToInt(Temperature.init.temperature);
        date.text = Tick.init.day + "/" + Tick.init.month + "/" + Tick.init.year;
    }
}
