/* Copyright (C) 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, March 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRateGoal : MonoBehaviour
{
    public int vSync;
    public int maxFps;

	private void Start ()
    {
        QualitySettings.vSyncCount = vSync;
        Application.targetFrameRate = maxFps;
    }
}
