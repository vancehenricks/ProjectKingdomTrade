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
        if (MapGenerator.init == null || Tick.init == null || Temperature.init == null) return "";

        return MapGenerator.init.width + "x" + MapGenerator.init.height + " " + MapGenerator.init.xOffset.ToString("0.00") + "," +
            MapGenerator.init.yOffset.ToString("0.00") + "," + MapGenerator.init.scale.ToString("0.00") + " " +
            Tick.init.day + "/" + Tick.init.month + "/" + Tick.init.year + " " +
            Tick.init.seconds + " " + Temperature.init.temperature + "c " + ClimateControl.init.Climate();
    }

    private void OnDestroy()
    {
        CDebug.extraParams = null;
    }

}
