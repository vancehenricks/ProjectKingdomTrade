/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreLoadPipeline : Pipeline
{
    private static bool runOnce;

    private void Update()
    {
        if (!runOnce)
        {
            runOnce = true;
            base.Execute();
        }
    }

    private new void OnDestroy()
    {
        runOnce = false;
        base.OnDestroy();
    }
}
