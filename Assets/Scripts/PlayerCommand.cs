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
        ExecuteCommands command = Command;
        CommandPipeline.Add(command, 100);
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
        unitInfos = Tools.Convert<TileInfo, UnitInfo>(MultiSelect.selectedTiles);
        MultiSelect.Clear(true);
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
        Debug.Log("END ACTION");
        CursorReplace.currentCursor = CursorType.Default;
        CursorReplace.SetCurrentCursorAsPrevious();
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
        foreach (List<TileInfo> _waypoints in _waypointList)
        {
            _waypoints.AddRange(waypoints);
        }
    }

    protected void AssignsToList(TileInfo waypoint, List<List<TileInfo>> _waypointList)
    {
        foreach (List<TileInfo> waypoints in _waypointList)
        {
            if (waypoint != null)
            {
                waypoints.Add(waypoint);
            }
        }
    }
}