/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, August 2019
 */


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
    public BoxCollider2D unitCollider2D;
    private bool saveCache;
    private UnitEffect unitEffect;
    private bool isPathFinding;
    private PathFinder pathFinder;
    private Coroutine transition;
    private Coroutine queueWayPoint;
    public bool inTransition;
    //private bool runOnceCheckNewWaypoint;

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
        if(transition != null)
        {
            StopCoroutine(transition);
        }
        isWalkable = null;
        wayPointReached = null;
        destinationChanged = null;
    }

    private void TickUpdate()
    {
        if (unitEffect == null && unitInfo.standingTile == null) return;

        if (arrivalTime > -2 && arrivalTime <= 0)
        {
            transition = StartCoroutine(Transition());
            //transform.position = tileDestination.transform.position;
        }

        if (arrivalTime > -1)
        {
            arrivalTime -= 0.25f;
            return;
        }

        CheckNewWaypoint(true);
        
    }


    private IEnumerator Transition()
    {
        inTransition = true;
        Vector3Int unitPos;
        Vector3Int desPos;
        unitCollider2D.enabled = false;
        Transform oldParent = transform.parent;
        transform.SetParent(oldParent.transform.parent);
        transform.SetAsLastSibling();

        do
        {
            transform.position = Vector3.MoveTowards(transform.position, tileDestination.transform.position, 5f);
            unitPos = Vector3Int.FloorToInt(transform.position);
            desPos = Vector3Int.FloorToInt(tileDestination.transform.position);
            CDebug.Log(this, "unitInfo.tileId=" + unitInfo.tileId + " unitPos=" + unitPos + " desPos=" + desPos);

            yield return null;
        }
        while (unitPos != desPos);

        transform.position = tileDestination.transform.position;
        transform.SetParent(oldParent);       
        unitCollider2D.enabled = true;
        //transition = null;
        arrivalTime = -2;
        inTransition = false;
    }

    public void QueueNewWayPoint()
    {
        if(queueWayPoint != null)
        {
            StopCoroutine(queueWayPoint);
        }

        queueWayPoint = StartCoroutine(QueueNewWayPointCoroutine());
    }

    private IEnumerator QueueNewWayPointCoroutine()
    {
        while(inTransition)
        {
            yield return null;
        }
        CheckNewWaypoint(false);
    }

    private void CheckNewWaypoint(bool increment = false)
    {
       // runOnceCheckNewWaypoint = true;
        if (unitInfo.waypoints.Count == 0) return;

        if (wayPointIndex >= unitInfo.waypoints.Count)
        {
            wayPointIndex = 0;
        }

        TileInfo point = unitInfo.waypoints[wayPointIndex];

        if (point == null) return;

        if (unitInfo.standingTile.tileLocation == point.tileLocation || isWalkable != null && !isWalkable(point))
        {
            CDebug.Log(this, "unitInfo.tileId=" + unitInfo.tileId + " end=" + point.tileLocation + " Waypoint reached!", LogType.Warning);
            if (wayPointReached != null)
            {
                wayPointReached(point);
            }

            if (increment)
            {
                if (unitInfo.waypoints.Count > 1)
                {
                    wayPointIndex++;
                }
                else
                {
                    unitInfo.waypoints.Clear();
                }
            }

            ResetGeneratedWaypoints();
        }
        
        if (isWalkable == null || isWalkable(point))
        {
            GeneratePath(unitInfo.standingTile, point, increment);

            if(isWalkable != null && !isWalkable(tileDestination))
            {
                ResetDestination();
            }
        }

        if (wayPointIndex >= unitInfo.waypoints.Count)
        {
            unitInfo.waypoints.Clear();
            wayPointIndex = 0;
        }

        //runOnceCheckNewWaypoint = false;
    }
    private void GeneratePath(TileInfo standingTile, TileInfo pointTileInfo, bool increment)
    {

        if (gwPointsIndex == 0 && !isPathFinding && pointTileInfo.tileLocation != unitInfo.tileLocation)
        {
            PathFindValues pathFindValues = new PathFindValues();
            pathFindValues.currentPoint = standingTile;
            pathFindValues.finalPoint = pointTileInfo;
            pathFindValues.isWalkable = isWalkable;
            pathFindValues.cache = PathFindingCache.init.cache;

            pathFindValues.onDoneCalculate = (List<TileInfo> _generatedWayPoints) => {
                generatedWayPoints = _generatedWayPoints;

                //this keyword does not work on anonymous functions
                CDebug.Log(nameof(PathFindingHandler), "unitInfo.tileId=" + unitInfo.tileId + 
                "Transferring generatedWayPoints.Count=" + generatedWayPoints.Count);

                saveCache = true; 
            };

            pathFinder.Set(pathFindValues);

            PathFindingQueue.init.Enqueue(pathFinder);

            isPathFinding = true;

            CDebug.Log(this, "unitInfo.tileId=" + unitInfo.tileId + " start=" + standingTile.tileLocation + 
                " end=" + pointTileInfo.tileLocation + " Generating Pathfinding.", LogType.Warning);
        }

        if(saveCache)
        {
            saveCache = false;
            if (generatedWayPoints.Count > 0)
            {
                CDebug.Log(this, "unitInfo.tileId=" + unitInfo.tileId + " Adding Cache start=" + generatedWayPoints[0].tileLocation + " end=" +
                    generatedWayPoints[generatedWayPoints.Count-1].tileLocation);

                PathFindingCache.init.Add(standingTile, pointTileInfo, generatedWayPoints);
            }
        }

        if (gwPointsIndex < generatedWayPoints.Count)
        {
            if (destinationChanged != null)
            {
                destinationChanged(gwPointsIndex, generatedWayPoints);
            }
            tileDestination = generatedWayPoints[gwPointsIndex];
            arrivalTime = tileDestination.travelTime - unitEffect.unitInfo.travelSpeed;

            if (increment)
            {
                gwPointsIndex++;
            }
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
