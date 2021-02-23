/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, March 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Walkable
{
    public string tile;
    public float temperature;

    public Walkable (string _tile, float _temperature)
    {
        tile = _tile;
        temperature = _temperature;
    }
}

public class NonWalkableTiles : MonoBehaviour
{
    public PathFinding pathFinding;

    private void Start()
    {
        pathFinding.isWalkable += isWalkable;
    }

    private bool isWalkable(TileInfo tile)
    {
        bool isWalkable = true;

        List<Walkable> nonWalkable = pathFinding.unitInfo.nonWalkable;

        for (int i = 0; i < nonWalkable.Count;i++)
        {
            if (tile.tileType == nonWalkable[i].tile && tile.localTemp > nonWalkable[i].temperature)
            {
                isWalkable = false;
            }
        }

        return isWalkable;
    }
}
