﻿/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, January 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollRectOverride : ScrollRect
{

    override protected void LateUpdate()
    {
        base.LateUpdate();

        if (this.verticalScrollbar)
        {
            this.verticalScrollbar.size = 0;
        }
    }

    override public void Rebuild(CanvasUpdate executing)
    {
        base.Rebuild(executing);

        if (this.verticalScrollbar)
        {
            this.verticalScrollbar.size = 0;
        }
    }
}
