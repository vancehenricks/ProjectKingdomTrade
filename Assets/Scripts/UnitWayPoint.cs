/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, Febuary 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitWayPoint : SelectTiles
{

    public UnitInfo unitInfo;
    //public LineRenderer lineRenderer;
    public PathFinding pathFinding;
    public CombatHandler combatHandler;
    public GameObject moveFlag;
    public GameObject attackFlag;

    //public int index;
    // public bool cleaned;

    protected new void Start()
    {
        if (pathFinding != null)
        {
            pathFinding.wayPointReached += WayPointReached;
            pathFinding.wayPointCountChange += WayPointCountChange;
            pathFinding.firstWayPointChange += FirstWayPointChange;
        }

        if (combatHandler != null)
        {
            combatHandler.targetCountChange += TargetCountChange;
            combatHandler.firstTargetChange += FirstTargetChange;
        }

        Initialize();
    }

    private void WayPointReached(TileInfo tileInfo)
    {
        if (tileInfo != null && flags.Count > 0 && unitInfo.targets.Count == 0)
        {
            RemoveFlag(tileInfo);
        }
    }

    private void FirstWayPointChange(TileInfo tileInfo)
    {
        if (unitInfo.targets.Count > 0) return;

        RemoveAllFlag();
        DrawFlag(tileInfo, moveFlag);
    }

    private void WayPointCountChange(TileInfo tileInfo)
    {
        if (unitInfo.targets.Count > 0 || unitInfo.waypoints.Count == 1) return;

        if (tileInfo == null)
        {
            RemoveAllFlag();
            return;
        }

        DrawFlag(tileInfo, moveFlag);
    }

    private void FirstTargetChange(TileInfo tileInfo)
    {
        RemoveAllFlag();
        DrawAndSyncFlag(tileInfo, attackFlag);
    }

    private void TargetCountChange(TileInfo tileInfo)
    {
        //if (unitInfo.targets.Count == 1) return; <- this will likely cause issue if commented out

        if (tileInfo == null)
        {
            RemoveAllFlag();
            return;
        }

        DrawAndSyncFlag(tileInfo, attackFlag);
    }

    public void DrawFlag(TileInfo waypoint, GameObject bFlag, bool syncColor = true)
    {
        DrawFlag(unitInfo, waypoint, bFlag, syncColor);
    }

    public void DrawAndSyncFlag(TileInfo waypoint, GameObject bFlag, bool syncColor = true)
    {
        DrawAndSyncFlag(unitInfo, waypoint, bFlag, syncColor);
    }

    public void RemoveFlag(TileInfo tile)
    {
        RemoveFlag(unitInfo, tile);
    }
}
