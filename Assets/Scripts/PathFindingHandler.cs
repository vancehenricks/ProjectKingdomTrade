/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
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
    public Destination destination;

    public delegate void WayPointReached(TileInfo tileInfo);
    public WayPointReached wayPointReached;

    public delegate void WayPointCountChange(TileInfo tileInfo);
    public WayPointCountChange wayPointCountChange;

    public delegate bool IsWalkable(TileInfo tile);
    public IsWalkable isWalkable;

    public delegate void FirstWayPointChange(TileInfo generatedWayPoint);
    public FirstWayPointChange firstWayPointChange;

    public List<TileInfo> generatedWayPoints;
    public int gwPointsIndex;

    private UnitEffect unitEffect;
    private int previousWayPointCount;
    private TileInfo firstWayPoint;
    private bool saveCache;

    private bool isPathFinding;
    private PathFinder pathFinder;

    private void Start()
    {
        destination = new Destination();
        destination.tile = unitInfo;
        destination.arrivalTime = -1;
        Tick.init.tickUpdate += TickUpdate;
        unitEffect = unitInfo.unitEffect;
        generatedWayPoints = new List<TileInfo>();
        firstWayPoint = unitInfo;
        pathFinder = new PathFinder();
    }

    public void OnDestroy()
    {
        Tick.init.tickUpdate -= TickUpdate;
        isWalkable = null;
        wayPointReached = null;
        wayPointCountChange = null;
        firstWayPointChange = null;
    }

    private void Update()
    {
        if (unitInfo.waypoints.Count > 0 && unitInfo.waypoints[0] != null && firstWayPoint.tileId != unitInfo.waypoints[0].tileId)
        {
            ResetGeneratedWaypoints();
            firstWayPoint = unitInfo.waypoints[0];

			if(firstWayPointChange != null)
			{
				firstWayPointChange(unitInfo.waypoints[0]);
			}
        }

        if (previousWayPointCount != unitInfo.waypoints.Count)
        {
            previousWayPointCount = unitInfo.waypoints.Count;

            TileInfo tile = null;

            if (unitInfo.waypoints.Count > 0)
            {
                tile = unitInfo.waypoints[previousWayPointCount - 1];
            }

			if(wayPointCountChange != null)
			{
				wayPointCountChange(tile);
			}
        }
    }

    private void TickUpdate()
    {
        if (unitEffect == null && destination == null) return;
        if (unitInfo.standingTile == null) return;

        if (destination.arrivalTime <= 0)
        {
            transform.position = destination.tile.transform.position;
        }

        if (destination.arrivalTime > -1)
        {
            destination.arrivalTime -= 0.25f;
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
                if (wayPointReached != null)
                {
                    CDebug.Log(this, "unitInfo.tileId =" + unitInfo.tileId + "end= " + point.tileLocation + "Waypoint reached!");
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
                GeneratePath(unitInfo.standingTile, point, destination);
            }

            if (wayPointIndex >= unitInfo.waypoints.Count)
            {
                unitInfo.waypoints.Clear();
                wayPointIndex = 0;
            }
        }
    }


    private void GeneratePath(TileInfo standingTile, TileInfo pointTileInfo, Destination destination)
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
            destination.tile = generatedWayPoints[gwPointsIndex];
            destination.arrivalTime = destination.tile.travelTime - unitEffect.unitInfo.travelSpeed;
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
        destination.tile = unitInfo;
        destination.arrivalTime = -1;
    }
}
