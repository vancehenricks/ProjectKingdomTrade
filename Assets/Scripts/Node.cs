﻿/* Copyright (C) 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, April 2020
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Node
{
    public TileInfo _tile, _origin, _target;
    public float _h, _g;
    public Node parent;
    //public List<Node> neighbours;
    public Dictionary<Vector2, Node> _closed;

    public float f
    {
        get { return _g + _h + _tile.travelTime; }
    }

    public Node(TileInfo tile, TileInfo origin, TileInfo target, Dictionary<Vector2, Node> closed, bool firstNode = false)
    {
        _origin = origin;
        _target = target;
        if (firstNode)
        {
            _g = Vector2.Distance(tile.tileLocation, origin.tileLocation);
            _h = Vector2.Distance(tile.tileLocation, target.tileLocation);
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

        foreach (TileInfo tile in Tools.GetNeighbours(_tile))
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

    public static Node GetLowestFCost(Dictionary<Vector2, Node> dNodes)
    {
        SortedDictionary<float, Node> sortedNodes = new SortedDictionary<float, Node>();

        foreach (Node n in dNodes.Values)
        {
            try
            {
                sortedNodes.Add(n.f, n);
            }
            catch { /*do nothing*/ }

        }

        return sortedNodes.Values.First();
    }
}
