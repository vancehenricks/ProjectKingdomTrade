/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, December 2020
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeHandler : MonoBehaviour
{
    public UnitInfo unitInfo;
    public PathFinding pathFinding;

    private bool isMerging;

    private void Start()
    {
        pathFinding.wayPointReached += WayPointReached;
    }

    private void OnDestroy()
    {
        pathFinding.wayPointReached -= WayPointReached;
    }

    public void GenerateWayPoint()
    {
        //MergeReset();
        unitInfo.waypoints.Add(unitInfo.merge.unitEffect.standingTile);
        isMerging = true;
    }

    private void WayPointReached(TileInfo tile)
    {
        if (!isMerging || unitInfo.merge == null)
        {
            MergeReset();
            return;
        }

        int distance = Tools.TileLocationDistance(unitInfo, unitInfo.merge);

        if (distance > 0)
        {
            GenerateWayPoint();
        }
        else if (distance == 0)
        {
            Tools.Merge(unitInfo, unitInfo.merge);
        }
    }

    private void MergeReset()
    {
        isMerging = false;
        unitInfo.merge = null;
    }
}
