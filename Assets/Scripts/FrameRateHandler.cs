/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, March 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRateHandler : MonoBehaviour
{
    private static FrameRateHandler _init;

    public static FrameRateHandler init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public bool _vSync;
    public int _maxFps;

    public bool vSync
    {
        get
        {
            return _vSync;
        }
        set
        {
            QualitySettings.vSyncCount = vSync == true ? 1 : 0;
        }
    }
    public int maxFps
    {
        get
        {
            return _maxFps;
        }
        set
        {
            _maxFps = value;
            Application.targetFrameRate = _maxFps;
        }
    }

    private void Awake()
    {
        init = this;
    }
}
