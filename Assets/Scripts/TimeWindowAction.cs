/* Copyright (C) 2019 by Vance Henricks Patual - All Rights Reserved
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

	private void Update ()
    {

		if(InputOverride.GetKeyUp(pauseKey))
        {
			PauseAction();
		}

		if(InputOverride.GetKeyUp(forwardKey))
        {
			ForwardAction();
		}

		if(InputOverride.GetKeyUp(backwardKey))
        {
			BackwardAction();
		}
	}

	public void PauseAction () {

		if(isPaused)
        {
			isPaused = false;
			Tick.speed = savedSpeed;
				
		}
        else
        {
			
			if(Tick.speed <= minSpeed)
            {
				Tick.speed = 1;
			}
            else
            {
				savedSpeed = Tick.speed;
				Tick.speed = minSpeed;
				isPaused = true;
			}
		}
	}

	public void ForwardAction ()
    {
		
		if(Tick.speed+speed <= maxSpeed)
        {
            Tick.speed += speed;
            isPaused = false;
        }
	}

	public void BackwardAction () {
		
		if(Tick.speed-speed >= minSpeed)
        {
            Tick.speed -= speed;
            isPaused = false;
        }
	}
}
