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
    public List<TileInfo> unitInfos;
    //public List<List<TileInfo>> waypointsList;
    //public List<List<TileInfo>> targetList;
    private float key;

    protected virtual void Start()
    {
        //waypointsList = new List<List<TileInfo>>();
        //targetList = new List<List<TileInfo>>();
    }

    protected virtual void Command()
    {

    }

    public virtual void DoAction()
    {
        key = CommandPipeline.init.Add(Command, 100);
        //waypointsList.Clear();
        //targetList.Clear();
        unitInfos = new List<TileInfo>(MultiSelect.init.selectedTiles);
        MultiSelect.init.Clear(true);
        OpenRightClick.init.ResetValues();

        //foreach (UnitInfo unit in unitInfos)
        //{
        //    waypointsList.Add(unit.waypoints);
        //    targetList.Add(unit.targets);
        //}
    }

    public virtual void EndAction()
    {
        CDebug.Log(this,"END ACTION");
        CommandPipeline.init.Remove(key);
        CursorReplace.init.currentCursor = CursorType.Default;
        CursorReplace.init.SetCurrentCursorAsPrevious();
        unitInfos.Clear();
    }

    protected void Execute(List<TileInfo> waypoints, string command)
    {
        for (int i=0;i < unitInfos.Count;i++)
        {
            ConsoleParser.init.ConsoleEvent(command, unitInfos[i], waypoints);  
        }      
    }

    protected void Execute(TileInfo waypoint, string command)
    {     
        List<TileInfo> tmp = new List<TileInfo>();
        tmp.Add(waypoint);

        Execute(tmp, command);
    }

    /*protected void UpdateUnitsWayPoints()
    {
        foreach (UnitInfo unit in unitInfos)
        {
            //unit.unitEffect.pathFinder.QueueNewWayPoint();
            unit.unitEffect.unitWayPoint.UpdateWayPoint();
        }
    }*/

    /*protected List<UnitInfo> ConvertToUnitInfo(List<TileInfo> tileInfos)
    {
        List<UnitInfo> unitInfos = new List<UnitInfo>();

        foreach (TileInfo tileInfo in tileInfos)
        {
            UnitInfo unitInfo = tileInfo as UnitInfo;

            if(unitInfo == null) continue;
            unitInfos.Add(unitInfo);
        }

        return unitInfos;
    }*/

}