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
    //public TileInfoRaycaster tileInfoRayCaster;
    //public TileInfoGetter pathFinder;
    public UnitInfo unitInfo;
    public int index;
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
    public int executeAlgorithmThreshold;

    public BoxCollider2D tileCollider;

    private UnitEffect unitEffect;
    private Task task;

    private int executeAlgorithmCounter;
    private int previousWayPointCount;

    private TileInfo firstWayPoint;
    private bool saveCache;

    private static string currentTileId;
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
        //getUpdatedWayPoints = null;
    }

    private void Update()
    {
        if (destination.arrivalTime <= -1)
        {
            transform.position = destination.tile.transform.position;
        }
        else if (destination.arrivalTime <= 0)
        {
            transform.position = Vector2.Lerp(transform.position, destination.tile.transform.position, 10f * Time.deltaTime);

            if (Vector2.Distance(transform.position, destination.tile.transform.position) < 0.5f)
            {
                tileCollider.enabled = true;
                destination.arrivalTime = -1;
            }
            else
            {
                tileCollider.enabled = false;
                unitEffect.OnExit();
            }

            transform.SetAsLastSibling();

        }

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

        if (destination.arrivalTime == -1 && destination.tile.tileLocation != unitInfo.standingTile.tileLocation)
        {
            transform.position = unitInfo.standingTile.transform.position;
            //Debug.Log("82");
        }

        if (destination.arrivalTime > -1)
        {
            //Debug.Log("91");
            destination.arrivalTime -= 0.25f;
        }
        else if (unitInfo.waypoints.Count > 0)
        {
            if (index >= unitInfo.waypoints.Count)
            {
                //Debug.Log("98");
                index = 0;
            }

            TileInfo point = unitInfo.waypoints[index];
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
                    index++;
                }
                else
                {
                    unitInfo.waypoints.Clear();
                }
                ResetGeneratedWaypoints();
                //Debug.Log("120");
            }

            if (isWalkable != null && isWalkable(point))
            {
                ExecuteAlgorithm(unitInfo.standingTile, point, destination);
            }

            //Debug.Log("133");

            if (index >= unitInfo.waypoints.Count)
            {
                //Debug.Log("138");
                unitInfo.waypoints.Clear();
                index = 0;
            }
        }
    }

    //+Not main thread
    private void OnDoneCalculate(List<TileInfo> _generatedWayPoints)
    {
        generatedWayPoints = _generatedWayPoints;
        saveCache = true;
    }

    private void AlgorithmicCounter(int counter)
    {
        executeAlgorithmCounter = counter;
    }
    //-Not main Thread

    private void ExecuteAlgorithm(TileInfo standingTile, TileInfo pointTileInfo, Destination destination)
    {

        //Debug.Log("136");
        if (currentTileId == "" && gwPointsIndex == 0 && task == null && pointTileInfo.tileLocation != unitInfo.tileLocation)
        {
            PathFindValues pathFindValues = new PathFindValues();

            pathFindValues.currentPoint = standingTile;
            pathFindValues.finalPoint = pointTileInfo;
            pathFindValues.isWalkable = isWalkable;
            pathFindValues.onDoneCalculate = OnDoneCalculate;
            pathFindValues.algorthimicCounter = AlgorithmicCounter;
            pathFindValues.tempCache = PathFindingCache.init.RetrieveTileInfos(standingTile, pointTileInfo);

            pathFinder.Set(pathFindValues);
            task = new Task(pathFinder.Calculate);
            task.Start();

            CDebug.Log(this, "unitInfo.tileId=" + unitInfo.tileId + " start=" + standingTile.tileLocation + "end=" + pointTileInfo.tileLocation + "Generating Pathfinding.");
            currentTileId = standingTile.tileId + ""; //this will force to only execute one calculation with same standingtile
        }
        else if (currentTileId == (standingTile.tileId + "") && generatedWayPoints.Count > 0)
        {
            currentTileId = "";
        }

        //Debug.Log("generatedWayPoints.Count=" + generatedWayPoints.Count);

        if (saveCache && generatedWayPoints.Count > 0 && !PathFindingCache.init.ContainsKey(standingTile, pointTileInfo))
        {
            saveCache = false;

            CDebug.Log(this,"Adding Cache start=" + generatedWayPoints[0].tileLocation + " end=" + generatedWayPoints[generatedWayPoints.Count-1].tileLocation);
            List<TileInfo> temp = new List<TileInfo>(generatedWayPoints);
            PathFindingCache.init.Add(standingTile, pointTileInfo, temp);
        }

        if (currentTileId != "") return;

        if (gwPointsIndex < generatedWayPoints.Count)
        {
            //Debug.Log("145");
            destination.tile = generatedWayPoints[gwPointsIndex];
            destination.arrivalTime = destination.tile.travelTime - unitEffect.unitInfo.travelSpeed;
            gwPointsIndex++;
            //Debug.Log("146");
        }
        else if (executeAlgorithmCounter > executeAlgorithmThreshold && generatedWayPoints.Count == 0)
        {
            CDebug.Log(this,"executeAlgorithmCounter=" + executeAlgorithmCounter);
            executeAlgorithmCounter = 0;
            index++;
        }
        else if (generatedWayPoints.Count == 0)
        {
            CDebug.Log(this,"executeAlgorithmCounter=" + executeAlgorithmCounter);
            executeAlgorithmCounter++;
        }
    }

    public void ResetGeneratedWaypoints()
    {
        currentTileId = "";
        gwPointsIndex = 0;
        StopAllCoroutines();
        task = null; 
        executeAlgorithmCounter = 0;
        generatedWayPoints.Clear();
    }

    public void ResetDestination()
    {
        destination.tile = unitInfo;
        destination.arrivalTime = -1;
    }
}
