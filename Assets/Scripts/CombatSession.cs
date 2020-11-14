/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, November 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSession : MonoBehaviour
{
    public UnitInfo unitInfo;
    [SerializeField]
    private List<TileInfo> combatants;

    private void Start()
    {
        combatants = new List<TileInfo>();
        combatants.Add(unitInfo);
        unitInfo.onEnd += OnEnd;
    }

    private void OnEnd()
    {
        for (int i = 1; i < combatants.Count; i++)
        {
            UnitInfo unit = combatants[i] as UnitInfo;
            CombatHandler unitCombatHandler = unit.unitEffect.combatHandler;

            unitCombatHandler.combatSession = null;
        }
    }

    public void Add(TileInfo tileInfo)
    {
        if (Tools.Exist(combatants, tileInfo)) return;

        combatants.Add(tileInfo);
    }

    public void Relay()
    {
        //PathFinding hostPathFinder = host.unitEffect.pathFinder;
        CombatHandler hostCombatHandler = unitInfo.unitEffect.combatHandler;
        TileInfoRaycaster tileInfoRaycaster = hostCombatHandler.tileInfoRaycaster;
        Camera cm = tileInfoRaycaster.cm;
        TileInfo point = unitInfo;
        int startingIndex = 1;

        if (unitInfo.currentTarget != null)
        {
            Vector3 centroid = Vector3.zero;

            foreach (UnitInfo unit in combatants)
            {
                centroid += unit.transform.position;
            }

            centroid /= combatants.Count;
            point =  tileInfoRaycaster.GetTileInfoFromPos(cm.WorldToScreenPoint(centroid));
            startingIndex = 0;
        }

        //Debug.Log("POINTS="+point.tileLocation);

        for (int i = startingIndex; i < combatants.Count; i++)
        {
            //PathFinding hostPathFinder = host.unitEffect.pathFinder;
            UnitInfo unit = combatants[i] as UnitInfo;
            CombatHandler unitCombatHandler = unit.unitEffect.combatHandler;

            unitCombatHandler.GenerateWaypoint(point);
        }
    }

    public void Clear()
    {
        combatants.Clear();
        combatants.Add(unitInfo);
    }
}