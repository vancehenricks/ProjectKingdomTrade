/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, March 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        if (tile.tileType == "Sea" && tile.localTemp > 0)
        {
            isWalkable = false;
        }

        return isWalkable;
    }
}
