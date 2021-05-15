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
    public PathFindingHandler pathFinding;
    public CombatHandler combatHandler;
    public GameObject moveFlag;
    public GameObject attackFlag;
    public GameObject mergeFlag;
    public GameObject directionFlag;

    //public int index;
    // public bool cleaned;

    protected new void Start()
    {
        if (pathFinding != null)
        {
            pathFinding.wayPointReached += WayPointReached;
            //pathFinding.destinationChanged += DestinationChanged;
        }

        if (combatHandler != null)
        {
            //combatHandler.targetCountChange += TargetCountChange;
            //combatHandler.firstTargetChange += FirstTargetChange;
        }

        Initialize();
    }

    protected override void OnDestroy()
    {
        if (pathFinding != null)
        {
            pathFinding.wayPointReached -= WayPointReached;
            //pathFinding.destinationChanged -= DestinationChanged;
        }

        if (combatHandler != null)
        {
            //combatHandler.targetCountChange -= TargetCountChange;
            //combatHandler.firstTargetChange -= FirstTargetChange;
        }

        base.OnDestroy();
    }

    public void UpdateWayPoint()
    {
        RemoveAllFlag();

        if (unitInfo.merge != null)
        {
            DrawAndSyncFlag(unitInfo.merge, mergeFlag);
        }
        else if (unitInfo.targets.Count == 0)
        {
            for (int i = 0; i < unitInfo.waypoints.Count; i++)
            {
                DrawFlag(unitInfo.waypoints[i], moveFlag);
            }
        }
    }

    private void WayPointReached(TileInfo tileInfo)
    {
        RemoveFlag(tileInfo);
    }

    /*private void DestinationChanged(int index, List<TileInfo> generatedTiles)
    {
        /*RemoveAllFlag();
        for (int i = index; i < generatedTiles.Count-1;i++)
        {
            DrawFlag(unitInfo, generatedTiles[i], directionFlag, true, false, false);
        }
    }*/

    public void DrawFlag(TileInfo waypoint, GameObject bFlag, bool syncColor = true)
    {
        DrawFlag(unitInfo, waypoint, bFlag, syncColor, true, false);
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
