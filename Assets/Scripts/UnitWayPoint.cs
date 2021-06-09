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
    public SyncIcon moveFlag;
    public SyncIcon attackFlag;
    public SyncIcon mergeFlag;
    public SyncIcon directionFlag;

    //public int index;
    // public bool cleaned;

    private void Start()
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

    private void OnDestroy()
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

        RemoveAllFlag();
    }

    public void UpdateWayPoint()
    {
        RemoveAllFlag();

        //issue with merging when drawing flags
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

        //SetDirectionFlag();
    }

    /*private void SetDirectionFlag()
    {
        int startIndex = pathFinding.gwPointsIndex - 1;

        if (startIndex < 0)
        {
            startIndex = pathFinding.gwPointsIndex;
        }

        /*for (int i = startIndex; i < pathFinding.generatedWayPoints.Count - 1; i++)
        {
            if (unitInfo.standingTile != null && unitInfo.standingTile.tileId == pathFinding.generatedWayPoints[i].tileId) continue;
            DrawAndSyncFlag(pathFinding.generatedWayPoints[i], directionFlag);
        }
    }*/

    private void WayPointReached(TileInfo tileInfo)
    {
        RemoveFlag(tileInfo, moveFlag);
        RemoveFlag(tileInfo, attackFlag);
        RemoveFlag(tileInfo, mergeFlag);
        //RemoveTypeFlag(directionFlag);
    }

    /*private void DestinationChanged(int index, List<TileInfo> generatedTiles)
    {
        if (unitInfo.selected)
        {
            RemoveTypeFlag(directionFlag);
            for (int i = index; i < generatedTiles.Count - 1; i++)
            {
                DrawAndSyncFlag(generatedTiles[i], directionFlag);
            }
        }
    }*/

    public void DrawFlag(TileInfo waypoint, SyncIcon bFlag, bool syncColor = true)
    {
        DrawFlag(unitInfo, waypoint, bFlag, syncColor);
    }

    public void DrawAndSyncFlag(TileInfo waypoint, SyncIcon bFlag, bool syncColor = true)
    {
        DrawAndSyncFlag(unitInfo, waypoint, bFlag, syncColor);
    }
}
