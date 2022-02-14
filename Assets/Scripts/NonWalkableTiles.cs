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
    public float minTemperature;
    public float maxTemperature;

    public Walkable (string _tile, float _minTemperature, float _maxTemperature)
    {
        tile = _tile;
        minTemperature = _minTemperature;
        maxTemperature = _maxTemperature;
    }
}

public class NonWalkableTiles : MonoBehaviour
{
    public PathFindingHandler pathFinding;

    private void Start()
    {
        pathFinding.isWalkable += isWalkable;
    }

    private void OnDestroy()
    {
        pathFinding.isWalkable -= isWalkable;
    }

    private bool isWalkable(TileInfo tile)
    {
        bool isWalkable = true;

        List<Walkable> nonWalkable = pathFinding.unitInfo.nonWalkable;

        for (int i = 0; i < nonWalkable.Count;i++)
        {
            if (tile.Contains(nonWalkable[i].tile) && tile.localTemp >= nonWalkable[i].minTemperature &&
                tile.localTemp <= nonWalkable[i].maxTemperature)
            {
                isWalkable = false;
            }
        }

        return isWalkable;
    }
}
