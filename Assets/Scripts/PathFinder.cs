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
    public TileInfo unitInfo;
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


        if (FinalizeTempCache(generatedWayPoints) || FindPath(generatedWayPoints))
        {
            pathFindValues.onDoneCalculate(generatedWayPoints);
        }
    }

    private bool FindPath(List<TileInfo> generatedWayPoints)
    {
        Dictionary<Vector2Int, Node> open = new Dictionary<Vector2Int, Node>();
        Dictionary<Vector2Int, Node> closed = new Dictionary<Vector2Int, Node>();

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

                CDebug.Log(this, "unitInfo.tileId=" + pathFindValues.unitInfo.tileId + 
                    " Done Pathfinding! generatedWaypoints.Count=" + generatedWayPoints.Count, LogType.Warning);
                
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

    private bool FinalizeTempCache(List<TileInfo> generatedWayPoints)
    {
        if (tempCache != null && tempCache.Count > 0 && CheckForWalkableTiles(tempCache))
        {
            generatedWayPoints.AddRange(tempCache);
            
            CDebug.Log(this, "unitInfo.tileid=" + pathFindValues.unitInfo.tileId + 
                " Re-using it tempCache.Count= " + tempCache.Count, LogType.Warning);

            return true;
        }       

        CDebug.Log(this, "unitInfo.tileid=" + pathFindValues.unitInfo.tileId + " Cache Invalid", LogType.Warning);

        return false;
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

    private bool CheckForWalkableTiles(List<TileInfo> tiles)
    {
        foreach (TileInfo tile in tiles)
        {
            if (pathFindValues.isWalkable != null && !pathFindValues.isWalkable(tile)) return false;
        }

        return true;
    }
}