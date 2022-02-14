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

    private int _speed;

    public int speed 
    {
        get 
        {
            return _speed;
        }

        set 
        {
            secondPerTick = 1f / value;
            _speed = value;
        }
    }

    public float secondPerTick { get; private set; }
    public long tick { get; private set; }
    public long day { get; private set; }
    public long month { get; private set; }
    public long year { get; private set; }
    public float timeDilation {get; private set; }

    public delegate void TickUpdate();
    public TickUpdate tickUpdate;

    private Coroutine startTick;

    private void Awake()
    {
        init = this;
    }

    public void Initialize()
    {
        timeDilation = 0.045f;
        tick = 5;
        speed = 1;
        day = 24;
        month = 8;
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

    private IEnumerator StartTick()
    {

        while (true)
        {

            if (speed != 0)
            {

                if (Tick.init.tickUpdate != null)
                {
                    Tick.init.tickUpdate();
                }

                tick++;

                if (tick > 24)
                {
                    day++;
                    tick = 1;
                }

                if (day > 30)
                {
                    month++;
                    day = 1;
                }

                if (month > 12)
                {
                    year++;
                    month = 1;
                }

                yield return new WaitForSecondsRealtime(secondPerTick);
            }
            
            yield return null;
            //CDebug.Log(this, "speed:" + speed + " secondPerTick:" + secondPerTick + " Tick:"+ tick + " Day:" +day+ " Month:" +month+ " Year:" +year, LogType.Warning);

        }
    }
}
