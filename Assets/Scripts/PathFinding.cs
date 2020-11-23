/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, August 2019
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinding : MonoBehaviour
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

    private UnitEffect unitEffect;
    private Coroutine coroutine;

    private int executeAlgorithmCounter;
    private int previousWayPointCount;

    private TileInfo firstWayPoint;

    private void Start()
    {
        destination = new Destination();
        destination.tile = unitInfo;
        destination.arrivalTime = -1;
        Tick.tickUpdate += TickUpdate;
        unitEffect = unitInfo.unitEffect;
        generatedWayPoints = new List<TileInfo>();
        firstWayPoint = unitInfo;
    }

    public void OnDestroy()
    {
        Tick.tickUpdate -= TickUpdate;
        isWalkable = null;
        wayPointReached = null;
        wayPointCountChange = null;
        //getUpdatedWayPoints = null;
    }

    private void Update()
    {
        if (destination.arrivalTime == 0)
        {
            transform.position = Vector2.Lerp(transform.position, destination.tile.transform.position, (10f * Tick.speed) * Time.deltaTime);
        }

        if (destination.arrivalTime == 0 && Vector2.Distance(transform.position, destination.tile.transform.position) < 0.5f)
        //if (destination.arrivalTime == 0 && unitInfo.tileLocation == destination.tile.tileLocation)
        {
            transform.position = destination.tile.transform.position;
            destination.arrivalTime = -1;
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
        if (unitEffect.standingTile == null) return;

        if (destination.arrivalTime == -1 && destination.tile.tileLocation != unitEffect.standingTile.tileLocation)
        {
            transform.position = unitEffect.standingTile.transform.position;
            //Debug.Log("82");
        }

        if (destination.arrivalTime > -1)
        {
            //Debug.Log("91");
            destination.arrivalTime--;
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

            if (unitEffect.standingTile.tileLocation == point.tileLocation || isWalkable != null && !isWalkable(point))
            {
                if (wayPointReached != null)
                {
                    Debug.Log("Waypoint reached");
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
                ExecuteAlgorithm(unitEffect.standingTile, point, destination);
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

    private void ExecuteAlgorithm(TileInfo standingTile, TileInfo pointTileInfo, Destination destination)
    {
        //Debug.Log("136");
        if (gwPointsIndex == 0 && coroutine == null && pointTileInfo.tileLocation != unitInfo.tileLocation)
        {
            coroutine = StartCoroutine(FindPath(standingTile, pointTileInfo));
            Debug.Log("generatedWayPoints.Count:" + generatedWayPoints.Count);
        }

        //Debug.Log("generatedWayPoints.Count=" + generatedWayPoints.Count);

        if (gwPointsIndex < generatedWayPoints.Count)
        {
            //Debug.Log("145");
            destination.tile = generatedWayPoints[gwPointsIndex];
            destination.arrivalTime = destination.tile.travelTime;
            gwPointsIndex++;
            //Debug.Log("146");
        }
        else if (executeAlgorithmCounter > executeAlgorithmThreshold && generatedWayPoints.Count == 0)
        {
            Debug.Log("178 executeAlgorithmCounter=" + executeAlgorithmCounter);
            executeAlgorithmCounter = 0;
            index++;
        }
        else if (generatedWayPoints.Count == 0)
        {
            Debug.Log("173 executeAlgorithmCounter=" + executeAlgorithmCounter);
            executeAlgorithmCounter++;
        }
    }

    public void ResetGeneratedWaypoints()
    {
        gwPointsIndex = 0;
        StopAllCoroutines();
        coroutine = null;
        executeAlgorithmCounter = 0;
        generatedWayPoints.Clear();
    }

    public void ResetDestination()
    {
        destination.tile = unitInfo;
        destination.arrivalTime = -1;
    }

    private IEnumerator FindPath(TileInfo currentPoint, TileInfo finalPoint)
    {
        if (currentPoint == null || finalPoint == null) yield break;

        Dictionary<Vector2, Node> open = new Dictionary<Vector2, Node>();
        Dictionary<Vector2, Node> closed = new Dictionary<Vector2, Node>();

        open.Add(currentPoint.tileLocation, new Node(currentPoint, currentPoint, finalPoint, closed, true));

        //f = h + g
        //g = distance from starting node
        //h = distance from end node
        //check for lowest f cost
        //if all f cost is equal find lowest h cost
        //if selected node has greater than equal f and g cost find lowest f cost
        //mark as explored any nodes which were already explored
        //h g f cost are always calculated the moment parent node is selected

        Debug.Log("213 open.Count=" + open.Count);

        while (open.Count > 0)
        {
            Node current = Node.GetLowestFCost(open);
            open.Remove(current._tile.tileLocation);

            //Debug.Log("220 open.Count:"+open.Count);

            closed.Add(current._tile.tileLocation, current);

            if (current._tile.tileLocation == finalPoint.tileLocation)
            {
                generatedWayPoints = current.GenerateWaypoints();
            }

            foreach (Node n in current.GetNeighbours())
            {
                //Debug.Log("checking neighbours");

                if (isWalkable != null)
                {
                    if (!isWalkable(n._tile) || closed.ContainsKey(n._tile.tileLocation)) continue;
                }

                if (!open.ContainsKey(n._tile.tileLocation))
                {
                    n._g = current._g + Vector2.Distance(current._tile.tileLocation, n._tile.tileLocation);
                    n._h = Vector2.Distance(n._tile.tileLocation, finalPoint.tileLocation);
                    n.parent = current;
                    open.Add(n._tile.tileLocation, n);
                }

            }

            executeAlgorithmCounter = 0;
        }

    }

    private bool HasSameLastTileInfo(List<TileInfo> tiles1, List<TileInfo> tiles2)
    {
        if (tiles1.Count > 0 && tiles2.Count > 0 && tiles1[tiles1.Count - 1].tileLocation == tiles2[tiles2.Count - 1].tileLocation)
        {
            return true;
        }

        return false;
    }

    private List<TileInfo> FitlerWaklableTilesOnly(List<TileInfo> tiles)
    {
        List<TileInfo> final = new List<TileInfo>();

        foreach (TileInfo tile in tiles)
        {
            if (isWalkable != null && !isWalkable(tile)) return final;

            Debug.Log("Checking for walkables");
            Debug.Log("tiles.count=" + tiles.Count);
            final.Add(tile);
        }

        return final;
    }
}
