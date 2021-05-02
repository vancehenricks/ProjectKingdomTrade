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
    public UnitInfo unitInfo;

    private void Start()
    {
        Tick.init.tickUpdate += TickUpdate;
    }

    private void OnDestroy()
    {
        Tick.init.tickUpdate -= TickUpdate;
    }

    private void TickUpdate()
    {
        if (pathfinding.destination.tile == null) return;
        Tools.SetDirection(transform, pathfinding.destination.tile.transform.position);
    }
}
