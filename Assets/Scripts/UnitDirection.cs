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
    public PathFindingHandler pathFindingHandler;
    public CombatHandler combatHandler;

    private void Start()
    {
        pathFindingHandler.destinationChanged += DestinationChanged;
        combatHandler.OnEnterCombat += OnEnterCombat;
    }

    private void OnDestroy()
    {
        pathFindingHandler.destinationChanged -= DestinationChanged;
        combatHandler.OnEnterCombat -= OnEnterCombat;
    }

    private void OnEnterCombat(UnitInfo origin, UnitInfo target)
    {
        Tools.SetDirection(origin.transform, target.transform.position);
    }

    private void DestinationChanged(int index, List<TileInfo> generatedWayPoints)
    {
        if (generatedWayPoints.Count == 0) return;
        TileInfo tile = generatedWayPoints[index];

        if (tile == null) return;
        Tools.SetDirection(transform, tile.transform.position);
    }
}
