/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2019
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeWindowAction : MonoBehaviour
{

    public int speed;
    public int maxSpeed;
    public int minSpeed;
    public KeyCode pauseKey;
    public KeyCode forwardKey;
    public KeyCode backwardKey;

    private int savedSpeed;
    private bool isPaused;

    private void Update()
    {

        if (InputOverride.init.GetKeyUp(pauseKey))
        {
            PauseAction();
        }

        if (InputOverride.init.GetKeyUp(forwardKey))
        {
            ForwardAction();
        }

        if (InputOverride.init.GetKeyUp(backwardKey))
        {
            BackwardAction();
        }
    }

    public void PauseAction()
    {
        if (isPaused)
        {
            isPaused = false;
            Tick.init.speed = savedSpeed;

        }
        else
        {

            if (Tick.init.speed <= minSpeed)
            {
                Tick.init.speed = 1;
            }
            else
            {
                savedSpeed = Tick.init.speed;
                Tick.init.speed = minSpeed;
                isPaused = true;
            }
        }

        CDebug.Log(this, "PauseAction Tick.speed=" + Tick.init.speed);
    }

    public void ForwardAction()
    {
        if (Tick.init.speed + speed <= maxSpeed)
        {
            Tick.init.speed += speed;
            isPaused = false;
        }

        CDebug.Log(this, "ForwardAction Tick.speed=" + Tick.init.speed);
    }

    public void BackwardAction()
    {
        if (Tick.init.speed - speed >= minSpeed)
        {
            Tick.init.speed -= speed;
            isPaused = false;
        }

        CDebug.Log(this, "BackwardAction Tick.speed=" + Tick.init.speed);
    }
}
