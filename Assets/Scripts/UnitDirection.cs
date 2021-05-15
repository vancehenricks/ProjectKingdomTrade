/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, Febuary 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDirection : MonoBehaviour
{
    public PathFindingHandler pathfinding;

    private void Start()
    {
        pathfinding.destinationChanged += DestinationChanged;
    }

    private void OnDestroy()
    {
        pathfinding.destinationChanged -= DestinationChanged;
    }

    private void DestinationChanged(int index, List<TileInfo> generatedWayPoints)
    {
        if (generatedWayPoints.Count == 0) return;
        TileInfo tile = generatedWayPoints[index];

        if (tile == null) return;
        Tools.SetDirection(transform, tile.transform.position);
    }
}
