/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, December 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeHandler : MonoBehaviour
{
    public UnitInfo unitInfo;
    public PathFinding pathFinding;

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
        unitInfo.waypoints.Add(unitInfo.merge);
    }

    //this does not work well if standing tile does not match likely use tickupdate instead and check for distance
    private void WayPointReached(TileInfo tile)
    {
        UnitInfo unit = tile as UnitInfo;

        if (unit == null)
        {
            unitInfo.merge = null;
        }
        else if (unit.tileId != unitInfo.merge.tileId)
        {
            GenerateWayPoint();
        }
        else if (unit.tileId == unitInfo.merge.tileId)
        {
            //do merge logic
            unitInfo.merge = null;
        }
    }
}
