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
        if (combatants.Contains(unitInfo)) return;

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
            point = GetTileFromPos(centroid);
            startingIndex = 0;
        }

        for (int i = startingIndex; i < combatants.Count; i++)
        {
            //PathFinding hostPathFinder = host.unitEffect.pathFinder;
            CombatHandler unitCombatHandler = combatants[i].unitEffect.combatHandler;
            unitCombatHandler.GenerateWaypoint(point);
        }
    }

    private TileInfo GetTileFromPos(Vector2 position)
    {
        Bounds bounds = new Bounds(position, new Vector2(3f,3f));
        TileInfo point = null;

        TileColliderHandler.init.Cast((List<BaseInfo> baseInfos) => {
            
            foreach(BaseInfo baseInfo in baseInfos)
            {
                point = baseInfo as TileInfo;
            }

        }, null, bounds, unitInfo.tileCollider.filterOut, 1, true);

        return point;
    }

    public void Clear()
    {
        combatants.Clear();
        combatants.Add(unitInfo);
    }
}