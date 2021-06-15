/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, August 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class PlayerCommand : MonoBehaviour
{
    public TileInfoRaycaster tileInfoRaycaster;
    public OpenRightClick openRightClick;
    public List<UnitInfo> unitInfos;
    public List<List<TileInfo>> waypointsList;
    public List<List<TileInfo>> targetList;

    protected virtual void Start()
    {
        CommandPipeline.init.Add(Command, 100);
        waypointsList = new List<List<TileInfo>>();
        targetList = new List<List<TileInfo>>();
    }

    protected virtual void Command()
    {

    }

    public virtual void DoAction()
    {
        waypointsList.Clear();
        targetList.Clear();
        unitInfos = ConvertToUnitInfo(MultiSelect.init.selectedTiles);
        MultiSelect.init.Clear(true);
        openRightClick.ResetValues();

        foreach (UnitInfo unit in unitInfos)
        {
            waypointsList.Add(unit.waypoints);
            targetList.Add(unit.targets);
        }

        ClearAllWaypoints();
    }

    public virtual void EndAction()
    {
        CDebug.Log(this,"END ACTION");
        CursorReplace.init.currentCursor = CursorType.Default;
        CursorReplace.init.SetCurrentCursorAsPrevious();
        unitInfos.Clear();
    }

    public virtual void ClearAllWaypoints()
    {

        foreach (List<TileInfo> target in targetList)
        {
            target.Clear();
        }

        foreach (UnitInfo unit in unitInfos)
        {
            if (unit != null)
            {
                unit.unitEffect.combatHandler.DisEngage();
                unit.merge = null;
            }
        }
    }

    protected void AssignsToList(List<TileInfo> waypoints, List<List<TileInfo>> _waypointList)
    {
        for (int i=0;i <  _waypointList.Count;i++)
        {
            _waypointList[i].AddRange(waypoints);
            unitInfos[i].unitEffect.unitWayPoint.UpdateWayPoint();
        }
    }

    protected void AssignsToList(TileInfo waypoint, List<List<TileInfo>> _waypointList)
    {
        for (int i=0;i < _waypointList.Count;i++)
        {
            if (waypoint != null)
            {
                _waypointList[i].Add(waypoint);
            }
            unitInfos[i].unitEffect.unitWayPoint.UpdateWayPoint();
        }
    }

    protected void UpdateUnitsWayPoints()
    {
        foreach (UnitInfo unit in unitInfos)
        {
            //unit.unitEffect.pathFinder.QueueNewWayPoint();
            unit.unitEffect.unitWayPoint.UpdateWayPoint();
        }
    }

    protected List<UnitInfo> ConvertToUnitInfo(List<TileInfo> tileInfos)
    {
        List<UnitInfo> unitInfos = new List<UnitInfo>();

        foreach (TileInfo tileInfo in tileInfos)
        {
            UnitInfo unitInfo = tileInfo as UnitInfo;

            if(unitInfo == null) continue;
            unitInfos.Add(unitInfo);
        }

        return unitInfos;
    }

}