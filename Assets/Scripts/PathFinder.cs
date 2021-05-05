/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, March 2021
 */

using DebugHandler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{
    public TileInfo currentPoint,finalPoint;
    public PathFindingHandler.IsWalkable isWalkable;
    public delegate void OnDoneCalculate(List<TileInfo> tiles);
    public OnDoneCalculate onDoneCalculate;
    public List<TileInfo> tempCache;

    public delegate void AlgorthimicCounter(int executeAlgorithmCounter);
    public AlgorthimicCounter algorthimicCounter;

    public void Set(TileInfo _currentPoint, TileInfo _finalPoint, List<TileInfo> _tempCache, PathFindingHandler.IsWalkable _isWalkable,
        OnDoneCalculate _onDoneCalculate, AlgorthimicCounter _algorthimicCounter)
    {

        currentPoint = _currentPoint;
        finalPoint = _finalPoint;
        isWalkable = _isWalkable;
        onDoneCalculate = _onDoneCalculate;
        algorthimicCounter = _algorthimicCounter;
        tempCache = _tempCache;
    }

    //f = h + g
    //g = distance from starting node
    //h = distance from end node
    //check for lowest f cost
    //if all f cost is equal find lowest h cost
    //if selected node has greater than equal f and g cost find lowest f cost
    //mark as explored any nodes which were already explored
    //h g f cost are always calculated the moment parent node is selected

    public void Calculate()
    {
        List<TileInfo> generatedWayPoints = new List<TileInfo>();

        if (currentPoint == null || finalPoint == null) return;

        if (tempCache != null)
        {
           CDebug.Log(this,"Found global cache re-using it tempCache.Count " + tempCache.Count);
        }


    REDO:
        Dictionary<Vector2Int, Node> open = new Dictionary<Vector2Int, Node>();
        Dictionary<Vector2Int, Node> closed = new Dictionary<Vector2Int, Node>();

        open.Add(currentPoint.tileLocation, new Node(currentPoint, currentPoint, finalPoint, closed, true));

        //CDebug.Log(this, "finalPoint.tileType=" + finalPoint.tileType, LogType.Warning);

        while (open.Count > 0)
        {

            //CDebug.Log(this,"Open.Count=" + open.Count, LogType.Warning);

            Node current = Node.GetLowestFCost(open);
            open.Remove(current._tile.tileLocation);

            //Debug.Log("220 open.Count:"+open.Count);

            closed.Add(current._tile.tileLocation, current);

            //CDebug.Log(this, "current._tile.tileLocation=" + current._tile.tileLocation + "finalPoint.tileLocation=" + finalPoint.tileLocation, LogType.Warning);

            if (current._tile.tileLocation == finalPoint.tileLocation /*|| tempCache != null*/)
            {
                generatedWayPoints = current.GenerateWaypoints();

                if (tempCache != null && tempCache.Count > 0)
                {
                    generatedWayPoints.AddRange(tempCache);
                    generatedWayPoints = FitlerWaklableTilesOnly(generatedWayPoints);

                    if (!HasSameLastTileInfo(generatedWayPoints, tempCache))
                    {
                        CDebug.Log(this,"CHECKING FAIL " + generatedWayPoints[generatedWayPoints.Count - 1].tileLocation + "!=" + finalPoint.tileLocation, LogType.Warning);
                        tempCache = generatedWayPoints;
                        generatedWayPoints = new List<TileInfo>();
                        currentPoint = generatedWayPoints[generatedWayPoints.Count - 1];
                        goto REDO;
                    }
                }

                CDebug.Log(this, "Done Pathfinding! generatedWaypoints.Count=" + generatedWayPoints.Count);

                break;
            }

            foreach (Node n in current.GetNeighbours())
            {
                //CDebug.Log(this,"Checking neighbours",LogType.Warning);

                if (isWalkable != null)
                {
                    if (!isWalkable(n._tile) || closed.ContainsKey(n._tile.tileLocation)) continue;
                }

                if (!open.ContainsKey(n._tile.tileLocation))
                {
                    n._g = current._g + Vector2.Distance(current._tile.tileLocation, n._tile.tileLocation);
                    n._h = Tools.TileLocationDistance(n._tile, finalPoint);
                    n.parent = current;
                    open.Add(n._tile.tileLocation, n);
                }
            }

            algorthimicCounter(0);
        }

        onDoneCalculate(generatedWayPoints);

        onDoneCalculate = null;
        algorthimicCounter = null;
		isWalkable = null; //might cause an issue
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

            //Debug.Log("Checking for walkables");
            //Debug.Log("tiles.count=" + tiles.Count);
            final.Add(tile);
        }

        return final;
    }
}