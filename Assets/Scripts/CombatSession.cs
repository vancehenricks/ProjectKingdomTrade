/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, November 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSession
{
    public List<TileInfo> combatants;

    public CombatSession()
    {
        combatants = new List<TileInfo>();
    }

    public void OnEnd()
    {
        foreach (UnitInfo unit in combatants)
        {
            CombatHandler unitCombatHandler = unit.unitEffect.combatHandler;
            unitCombatHandler.combatSession = null;
        }
    }

    public TileInfo GetPosition()
    {
        UnitInfo host = combatants[0] as UnitInfo;
        PathFinding hostPathFinder = host.unitEffect.pathFinder;
        CombatHandler hostCombatHandler = host.unitEffect.combatHandler;
        TileInfoRaycaster tileInfoRaycaster = hostCombatHandler.tileInfoRaycaster;
        Camera cm = tileInfoRaycaster.cm;
        Vector3 centroid = Vector3.zero;

        foreach (UnitInfo unit in combatants)
        {
            centroid += unit.transform.position;
        }

        centroid /= combatants.Count;

        TileInfo point = tileInfoRaycaster.GetTileInfoFromPos(cm.WorldToScreenPoint(centroid));

        //Debug.Log("POINTS="+point.tileLocation);

        return point;
    }
}