/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, April 2021
 */

using DebugHandler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugExtra : MonoBehaviour
{
    private void Awake()
    {
        CDebug.extraParams = ExtraParams;
    }

    private string ExtraParams()
    {
        return "TickDate="+Tick.init.day + "/" + Tick.init.month + "/" + Tick.init.year + 
            " Tick=" + Tick.init.seconds + " Temp=" + Temperature.init.temperature + " " + ClimateControl.init.Climate();
    }

    private void OnDestroy()
    {
        CDebug.extraParams = null;
    }

}
