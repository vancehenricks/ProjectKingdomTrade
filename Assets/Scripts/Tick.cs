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
    private static Tick _init;

    public static Tick init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public int speed;
    public float realSpeed { get; private set; }
    public float dayToSecond { get; private set; }
    public double realSeconds { get; private set; }
    public float seconds { get; private set; }
    public int day { get; private set; }
    public long realDays { get; private set; }
    public int month { get; private set; }
    public long realMonths { get; private set; }
    public int year { get; private set; }

    public delegate void TickUpdate();
    public TickUpdate tickUpdate;

    private Coroutine startTick;

    private void Awake()
    {
        init = this;
    }

    public void Initialize()
    {
        realSeconds = 5f;
        seconds = 5f;
        speed = 1;
        dayToSecond = 24f;
        day = 24;
        realDays = 24;
        month = 8;
        realMonths = 8;
        year = 500;
        startTick = StartCoroutine(StartTick());
    }

    private void OnDestroy()
    {
        if (startTick != null)
        {
            StopCoroutine(startTick);
        }
        tickUpdate = null;
    }

    IEnumerator StartTick()
    {

        while (true)
        {

            if (speed != 0)
            {

                realSeconds++;

                if (Tick.init.tickUpdate != null)
                {
                    Tick.init.tickUpdate();
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
            yield return new WaitForSecondsRealtime(realSpeed);
        }
    }
}
