/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, April 2020
 */

using DebugHandler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node
{
    public TileInfo _tile, _origin, _target;
    public float _h, _g;
    public Node parent;
    //public List<Node> neighbours;
    public Dictionary<Vector2Int, Node> _closed;

    public float f
    {
        get { return _g + _h + _tile.travelTime; }
    }

    public Node(TileInfo tile, TileInfo origin, TileInfo target, Dictionary<Vector2Int, Node> closed, bool firstNode = false)
    {
        _origin = origin;
        _target = target;
        if (firstNode)
        {
            _g = Tools.TileLocationDistance(tile, origin);
            _h = Tools.TileLocationDistance(tile, target);
        }
        _tile = tile;
        _closed = closed;
    }

    //traverses each node to generate a list
    public List<TileInfo> GenerateWaypoints()
    {
        List<TileInfo> waypoints = new List<TileInfo>();

        Node tNode = this;

        while (tNode.parent != null)
        {
            waypoints.Add(tNode._tile);
            tNode = tNode.parent;
        }

        waypoints.Reverse();

        return waypoints;
    }

    public List<Node> GetNeighbours()
    {
        List<Node> neighbours = new List<Node>();

        foreach (TileInfo tile in TileList.init.GetNeighbours(_tile))
        {
            if (parent == null || parent != null && tile.tileLocation != parent._tile.tileLocation)
            {
                Node n;
                if (!_closed.TryGetValue(tile.tileLocation, out n))
                {
                    n = new Node(tile, _origin, _target, _closed);
                }
                neighbours.Add(n);
            }
        }

        return neighbours;
    }

    public static Node GetLowestFCost(Dictionary<Vector2Int, Node> dNodes)
    {
        SortedDictionary<float, Node> sortedNodes = new SortedDictionary<float, Node>();

        foreach (Node n in dNodes.Values)
        {
            try
            {
                sortedNodes.Add(n.f, n);
            }
            catch (System.Exception e)
            {
                CDebug.Log(nameof(Node),e);
            }
        }

        return sortedNodes.Values.First();
    }
}
