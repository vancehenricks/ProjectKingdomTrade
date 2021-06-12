/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, March 2021
 */


using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class PathFindValues
{
    public TileInfo currentPoint;
    public TileInfo finalPoint;
    public ConcurrentDictionary<string, List<TileInfo>> cache;
    public PathFindingHandler.IsWalkable isWalkable;
    public System.Action<List<TileInfo>> onDoneCalculate;
}

public class PathFinder
{
    private PathFindValues pathFindValues;
    private List<TileInfo> tempCache;
    public void Set(PathFindValues _pathFindValues)
    {
        pathFindValues = _pathFindValues;
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
        if (pathFindValues.currentPoint == null || pathFindValues.finalPoint == null) return;
        
        List<TileInfo> generatedWayPoints = new List<TileInfo>();
        tempCache = RetrieveTileInfos(pathFindValues.currentPoint, pathFindValues.finalPoint);

        if (tempCache != null)
        {
            CDebug.Log(this, "Found global cache re-using it tempCache.Count " + tempCache.Count, LogType.Warning);
        }

        bool isDone = false;

        while (!isDone)
        {
            Dictionary<Vector2Int, Node> open = new Dictionary<Vector2Int, Node>();
            Dictionary<Vector2Int, Node> closed = new Dictionary<Vector2Int, Node>();

            isDone = FindPath(open, closed, ref generatedWayPoints);
        }

        pathFindValues.onDoneCalculate(generatedWayPoints);
    }

    private bool FindPath(Dictionary<Vector2Int, Node> open, Dictionary<Vector2Int, Node> closed, ref List<TileInfo> generatedWayPoints)
    {

        open.Add(pathFindValues.currentPoint.tileLocation, 
            new Node(pathFindValues.currentPoint, pathFindValues.currentPoint, pathFindValues.finalPoint, closed, true));

        while (open.Count > 0)
        {

            Node current = Node.GetLowestFCost(open);
            open.Remove(current._tile.tileLocation);
            closed.Add(current._tile.tileLocation, current);

            if (current._tile.tileLocation == pathFindValues.finalPoint.tileLocation)
            {
                generatedWayPoints = current.GenerateWaypoints();

                if (tempCache != null && tempCache.Count > 0)
                {
                    generatedWayPoints.AddRange(tempCache);
                    generatedWayPoints = FitlerWaklableTilesOnly(generatedWayPoints);

                    if (!HasSameLastTileInfo(generatedWayPoints, tempCache))
                    {
                        CDebug.Log(this, "Checking fail! " + generatedWayPoints[generatedWayPoints.Count - 1].tileLocation +
                            "!=" + pathFindValues.finalPoint.tileLocation + " generating another one.", LogType.Warning);

                        tempCache = generatedWayPoints;
                        pathFindValues.currentPoint = generatedWayPoints[generatedWayPoints.Count - 1];
                        generatedWayPoints = new List<TileInfo>();

                        return false;
                    }
                }

                CDebug.Log(this, "Done Pathfinding! generatedWaypoints.Count=" + generatedWayPoints.Count, LogType.Warning);
                return true;
            }

            foreach (Node n in current.GetNeighbours())
            {
                if (pathFindValues.isWalkable != null)
                {
                    if (!pathFindValues.isWalkable(n._tile) || closed.ContainsKey(n._tile.tileLocation)) continue;
                }

                if (!open.ContainsKey(n._tile.tileLocation))
                {
                    n._g = current._g + Vector2.Distance(current._tile.tileLocation, n._tile.tileLocation);
                    n._h = Tools.TileLocationDistance(n._tile, pathFindValues.finalPoint);
                    n.parent = current;
                    open.Add(n._tile.tileLocation, n);
                }
            }
        }

        return true;
    }

    private List<TileInfo> RetrieveTileInfos(TileInfo startPoint, TileInfo endPoint)
    {
        Vector2Int _start = startPoint.tileLocation;
        Vector2Int _end = endPoint.tileLocation;

        string keyword = _start + "," + _end;

        if (pathFindValues.cache.ContainsKey(keyword))
        {
            return pathFindValues.cache[keyword];
        }

        return null;
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
            if (pathFindValues.isWalkable != null && !pathFindValues.isWalkable(tile)) return final;

            final.Add(tile);
        }

        return final;
    }
}