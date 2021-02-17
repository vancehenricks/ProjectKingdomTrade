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
    private List<UnitInfo> combatants;

    private void Start()
    {
        combatants = new List<UnitInfo>();
    }

    private void OnDestroy()
    {
        for (int i = 1; i < combatants.Count; i++)
        {
            CombatHandler unitCombatHandler = combatants[i].unitEffect.combatHandler;
            unitCombatHandler.combatSession = null;
        }
    }

    public void Add(UnitInfo unitInfo)
    {
        if (Tools.Exist(combatants, unitInfo) > -1) return;

        combatants.Add(unitInfo);
    }

    public void Remove(UnitInfo unitInfo)
    {
        combatants.Remove(unitInfo);
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
            CombatHandler unitCombatHandler = combatants[i].unitEffect.combatHandler;
            unitCombatHandler.GenerateWaypoint(point);
        }
    }

    public void Clear()
    {
        combatants.Clear();
        combatants.Add(unitInfo);
    }
}