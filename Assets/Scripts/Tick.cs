/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, January 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tick : MonoBehaviour
{

    public static int speed;
    public static float realSpeed { get; private set; }
    public static float dayToSecond { get; private set; }
    public static double realSeconds { get; private set; }
    public static float seconds { get; private set; }
    public static int day { get; private set; }
    public static long realDays { get; private set; }
    public static int month { get; private set; }
    public static long realMonths { get; private set; }
    public static int year { get; private set; }

    public delegate void TickUpdate();
    public static TickUpdate tickUpdate;

    public void Initialize()
    {
        StopAllCoroutines();
        realSeconds = 5f;
        seconds = 5f;
        speed = 1;
        dayToSecond = 24f;
        day = 24;
        realDays = 24;
        month = 8;
        realMonths = 8;
        year = 500;
        StartCoroutine(startTick());
    }

    private void OnDestroy()
    {
        tickUpdate = null;
    }

    IEnumerator startTick()
    {

        while (true)
        {

            if (speed != 0)
            {

                realSeconds++;

                if (Tick.tickUpdate != null)
                {
                    Tick.tickUpdate();
                }

                seconds++;
                realSpeed = 1f / speed;

                if (seconds >= dayToSecond + 1)
                {
                    day++;
                    realDays++;
                    seconds = 1;
                }

                if (day >= 30)
                {
                    month++;
                    realMonths++;
                    day = 1;
                }

                if (month >= 12)
                {
                    year++;
                    month = 1;
                }
            }

            //Debug.Log("Seconds: "+ seconds + " Day:" +day+ " Month:" +month+ " Year:" +year);
            yield return new WaitForSeconds(realSpeed);
        }
    }
}
