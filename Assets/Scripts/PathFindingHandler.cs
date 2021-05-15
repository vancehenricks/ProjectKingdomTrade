﻿/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, August 2019
 */

using DebugHandler;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PathFindingHandler : MonoBehaviour
{
    public UnitInfo unitInfo;
    public int wayPointIndex;

    public delegate void WayPointReached(TileInfo tileInfo);
    public WayPointReached wayPointReached;

    public delegate void DestinationChanged(int currentIndex, List<TileInfo> generatedWayPoints);
    public DestinationChanged destinationChanged;

    public delegate bool IsWalkable(TileInfo tile);
    public IsWalkable isWalkable;

    public List<TileInfo> generatedWayPoints;
    public int gwPointsIndex;

    public TileInfo tileDestination;
    public float arrivalTime;

    private bool saveCache;
    private UnitEffect unitEffect;
    private bool isPathFinding;
    private PathFinder pathFinder;

    private void Start()
    {
        Tick.init.tickUpdate += TickUpdate;
        unitEffect = unitInfo.unitEffect;
        generatedWayPoints = new List<TileInfo>();
        pathFinder = new PathFinder();
        ResetDestination();
        ResetGeneratedWaypoints();
    }

    public void OnDestroy()
    {
        Tick.init.tickUpdate -= TickUpdate;
        isWalkable = null;
        wayPointReached = null;
        destinationChanged = null;
    }

    private void TickUpdate()
    {
        if (unitEffect == null && unitInfo.standingTile == null) return;

        if (arrivalTime > -2 && arrivalTime <= 0)
        {
            transform.position = tileDestination.transform.position;
        }

        if (arrivalTime > -1)
        {
            arrivalTime -= 0.25f;
        }
        else if (unitInfo.waypoints.Count > 0)
        {
            if (wayPointIndex >= unitInfo.waypoints.Count)
            {
                wayPointIndex = 0;
            }

            TileInfo point = unitInfo.waypoints[wayPointIndex];

            if (point == null) return;

            if (unitInfo.standingTile.tileLocation == point.tileLocation || isWalkable != null && !isWalkable(point))
            {
                CDebug.Log(this, "unitInfo.tileId =" + unitInfo.tileId + "end= " + point.tileLocation + "Waypoint reached!", LogType.Warning);
                if (wayPointReached != null)
                {
                    wayPointReached(point);
                }

                if (unitInfo.waypoints.Count > 1)
                {
                    wayPointIndex++;
                }
                else
                {
                    unitInfo.waypoints.Clear();
                }
                ResetGeneratedWaypoints();
            }

            if (isWalkable != null && isWalkable(point))
            {
                GeneratePath(unitInfo.standingTile, point);
            }

            if (wayPointIndex >= unitInfo.waypoints.Count)
            {
                unitInfo.waypoints.Clear();
                wayPointIndex = 0;
            }
        }
    }


    private void GeneratePath(TileInfo standingTile, TileInfo pointTileInfo)
    {

        if (gwPointsIndex == 0 && !isPathFinding && pointTileInfo.tileLocation != unitInfo.tileLocation)
        {
            PathFindValues pathFindValues = new PathFindValues();
            pathFindValues.currentPoint = standingTile;
            pathFindValues.finalPoint = pointTileInfo;
            pathFindValues.isWalkable = isWalkable;

            pathFindValues.onDoneCalculate = (List<TileInfo> _generatedWayPoints) => {
                generatedWayPoints = _generatedWayPoints;
                saveCache = true; 
            };

            pathFindValues.tempCache = PathFindingCache.init.RetrieveTileInfos(standingTile, pointTileInfo);
            pathFinder.Set(pathFindValues);

            PathFindingQueue.init.Push(pathFinder);

            isPathFinding = true;

            CDebug.Log(this, "unitInfo.tileId=" + unitInfo.tileId + " start=" + standingTile.tileLocation + 
                "end=" + pointTileInfo.tileLocation + "Generating Pathfinding.");
        }

        if (saveCache && generatedWayPoints.Count > 0)
        {
            saveCache = false;

            CDebug.Log(this,"Adding Cache start=" + generatedWayPoints[0].tileLocation + " end=" +
                generatedWayPoints[generatedWayPoints.Count-1].tileLocation);

            List<TileInfo> temp = new List<TileInfo>(generatedWayPoints);
            PathFindingCache.init.Add(standingTile, pointTileInfo, temp);
        }

        if (gwPointsIndex < generatedWayPoints.Count)
        {
            if (destinationChanged != null)
            {
                destinationChanged(gwPointsIndex, generatedWayPoints);
            }
            tileDestination = generatedWayPoints[gwPointsIndex];
            arrivalTime = tileDestination.travelTime - unitEffect.unitInfo.travelSpeed;
            gwPointsIndex++;
        }
    }

    public void ResetGeneratedWaypoints()
    {
        gwPointsIndex = 0;
        generatedWayPoints.Clear();
        isPathFinding = false;
    }

    public void ResetDestination()
    {
        tileDestination = unitInfo.standingTile == null ? unitInfo : unitInfo.standingTile;
        arrivalTime = -2;
    }
}
