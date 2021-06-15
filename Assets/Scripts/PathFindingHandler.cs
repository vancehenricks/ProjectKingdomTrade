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
    //private bool runOnceCheckNewWaypoint;
    private TileInfo firstWayPoint;

    private void Start()
    {
        Tick.init.tickUpdate += TickUpdate;
        unitEffect = unitInfo.unitEffect;
        generatedWayPoints = new List<TileInfo>();
        pathFinder = new PathFinder();
        firstWayPoint = unitInfo;
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
        if (unitEffect == null || unitInfo.standingTile == null || transition != null) return;

        if (arrivalTime > -2 && arrivalTime <= 0)
        {
            transition = StartCoroutine(Transition());
            return;
            //transform.position = tileDestination.transform.position;
        }

        if (arrivalTime > -1)
        {
            arrivalTime -= 0.25f;
            return;
        }

        CheckNewWaypoint();
    }


    private IEnumerator Transition()
    {
        //Tick.init.tickUpdate -= TickUpdate;
        Vector3Int unitPos;
        Vector3Int desPos;
        unitCollider2D.enabled = false;
        Transform previousParent = transform.parent;
        transform.SetParent(transform.parent.transform.parent);
        //transform.SetAsLastSibling();
        do
        {
            transform.position = Vector3.MoveTowards(transform.position, tileDestination.transform.position, 5f);
            unitPos = Vector3Int.FloorToInt(transform.position);
            desPos = Vector3Int.FloorToInt(tileDestination.transform.position);
            CDebug.Log(this, "unitInfo.tileId=" + unitInfo.tileId + " wayPointIndex=" + wayPointIndex + 
             " start=" + unitInfo.tileLocation + " end=" + tileDestination.tileLocation +
             " unitPos=" + unitPos + " desPos=" + desPos, LogType.Warning);

            yield return null;
        }
        while (unitPos != desPos);

        transform.position = tileDestination.transform.position;
        transform.SetParent(previousParent);     
        unitCollider2D.enabled = true;
        //yield return new WaitForEndOfFrame();
        transition = null;
        ResetDestination();
    }

    private void CheckNewWaypoint()
    {
       // runOnceCheckNewWaypoint = true;
        if (unitInfo.waypoints.Count == 0) return;

        if(firstWayPoint.tileId != unitInfo.waypoints[0].tileId)
        {
            CDebug.Log(this, "unitInfo.tileId=" + unitInfo.tileId + " wayPointIndex=" + wayPointIndex + 
                " firstWayPoint=" + firstWayPoint.tileLocation, LogType.Warning);

            wayPointIndex = 0;
            firstWayPoint = unitInfo.waypoints[0];
        }

        TileInfo point = unitInfo.waypoints[wayPointIndex];

        if (point == null) return;

        if (unitInfo.standingTile.tileLocation == point.tileLocation || isWalkable != null && !isWalkable(point))
        {
            CDebug.Log(this, "unitInfo.tileId=" + unitInfo.tileId + " wayPointIndex=" + wayPointIndex + 
                " end=" + point.tileLocation + " Waypoint reached!", LogType.Warning);

            if (wayPointReached != null)
            {
                wayPointReached(point);
            }

            ResetGeneratedWaypoints();
            //ResetDestination();

            if(++wayPointIndex >= unitInfo.waypoints.Count)
            {
                ResetWayPoints();
                ResetDestination();
                return;
            }
        }
        
        if (isWalkable == null || isWalkable(point))
        {
            GeneratePath(unitInfo.standingTile, point);

            if(isWalkable != null && !isWalkable(tileDestination))
            {
                if (wayPointReached != null)
                {
                    wayPointReached(point);
                }
                ResetGeneratedWaypoints();
                ResetDestination();
                ResetWayPoints();               
            }
        }
    }
    
    private void GeneratePath(TileInfo standingTile, TileInfo pointTileInfo)
    {

        if (gwPointsIndex == 0 && !isPathFinding && pointTileInfo.tileLocation != unitInfo.tileLocation)
        {
            PathFindValues pathFindValues = new PathFindValues();
            pathFindValues.unitInfo = unitInfo;
            pathFindValues.currentPoint = standingTile;
            pathFindValues.finalPoint = pointTileInfo;
            pathFindValues.isWalkable = isWalkable;
            pathFindValues.cache = PathFindingCache.init.cache;

            pathFindValues.onDoneCalculate = (List<TileInfo> _generatedWayPoints) => {
                generatedWayPoints = _generatedWayPoints;

                //this keyword does not work on anonymous functions
                CDebug.Log(nameof(PathFindingHandler), "unitInfo.tileId=" + unitInfo.tileId + " wayPointIndex=" + wayPointIndex + 
                    " generatedWayPoint.start=" + generatedWayPoints[0].tileLocation + " start=" + standingTile.tileLocation + 
                    " end=" + generatedWayPoints[generatedWayPoints.Count-1].tileLocation + 
                    " Transferring generatedWayPoints.Count=" + generatedWayPoints.Count, LogType.Warning);

                saveCache = true; 
            };

            pathFinder.Set(pathFindValues);

            PathFindingQueue.init.Enqueue(pathFinder);

            isPathFinding = true;

            CDebug.Log(this, "unitInfo.tileId=" + unitInfo.tileId + " wayPointIndex=" + wayPointIndex + " start=" + standingTile.tileLocation + 
                " end=" + pointTileInfo.tileLocation + " Generating Pathfinding.", LogType.Warning);
        }

        if(saveCache)
        {
            saveCache = false;
            if (generatedWayPoints.Count > 0)
            {
                CDebug.Log(this, "unitInfo.tileId=" + unitInfo.tileId + " wayPointIndex=" + wayPointIndex +
                    " Adding Cache start=" + standingTile.tileLocation + " end=" +
                    generatedWayPoints[generatedWayPoints.Count-1].tileLocation, LogType.Warning);

                PathFindingCache.init.Add(standingTile, generatedWayPoints);
            }
        }

        if (gwPointsIndex < generatedWayPoints.Count)
        {
            if (destinationChanged != null)
            {
                destinationChanged(gwPointsIndex, generatedWayPoints);
            }
            
            CDebug.Log(this, "unitInfo.tileId=" + unitInfo.tileId  + " gwPointsIndex=" + gwPointsIndex +
                " wayPointIndex=" + wayPointIndex +  " tileDestination=" + generatedWayPoints[gwPointsIndex].tileLocation + 
                " end=" + pointTileInfo.tileLocation, LogType.Warning);

            tileDestination = generatedWayPoints[gwPointsIndex];
            arrivalTime = tileDestination.travelTime - unitEffect.unitInfo.travelSpeed;

            gwPointsIndex++;
        }
    }

    public void ResetGeneratedWaypoints()
    {
        CDebug.Log(this, "unitInfo.tileId=" + unitInfo.tileId + " ResetGeneratedWaypoints", LogType.Warning);           
        gwPointsIndex = 0;
        generatedWayPoints.Clear();
        isPathFinding = false;
    }

    public void ResetDestination()
    {
        CDebug.Log(this, "unitInfo.tileId=" + unitInfo.tileId + " ResetDestination", LogType.Warning);        
        tileDestination = unitInfo.standingTile == null ? unitInfo : unitInfo.standingTile;
        arrivalTime = -2;
    }

    public void ResetWayPoints()
    {
        CDebug.Log(this, "unitInfo.tileId=" + unitInfo.tileId + " ResetWayPoints", LogType.Warning);
        unitInfo.waypoints.Clear();
        wayPointIndex = 0;     
    }
}
