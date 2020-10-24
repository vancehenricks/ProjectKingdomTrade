/* Copyright (C) 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, August 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MoveUnit : MonoBehaviour
{

    public List<UnitInfo> unitInfos;
    public TileInfoRaycaster tileInfoRaycaster;
    public OpenRightClick openRightClick;
    public List<List<TileInfo>> waypointsList;
    public List<List<TileInfo>> targetList;

    protected virtual void Start()
    {
        waypointsList = new List<List<TileInfo>>();
        targetList = new List<List<TileInfo>>();
        ExecuteCommands command = Command;
        CommandPipeline.Add(command, 100);

        DragHandler.overrideOnBeginDrag += OverrideOnBeginDrag;
    }

    protected virtual void Command()
    {
        //Debug.Log("MoveUnit");

        if (unitInfos.Count == 0) return;

        if (Input.GetButtonDown("Fire1") && MultiSelect.shiftPressed)
        {
            Debug.Log("Creating multiple waypoint...");
            AssignsToList(tileInfoRaycaster.GetTileInfoFromPos(Input.mousePosition), waypointsList);
            //shiftSelected = false;
        }

        if (Input.GetButtonDown("Fire1") && !MultiSelect.shiftPressed)
        {
            Debug.Log("Creating one waypoint... unitInfos.Count="+ unitInfos.Count);
            TileInfo tileInfo = tileInfoRaycaster.GetTileInfoFromPos(Input.mousePosition);
            ClearAllWaypoints();
            AssignsToList(tileInfo, waypointsList);
        } 

        if (Input.GetButtonDown("Fire2"))
        {
            //OptionGenerator.blockDisplay = true;
            openRightClick.doNotDisplay = true;
            EndAction();
        }
    }

    private void OverrideOnBeginDrag(PointerEventData eventData)
    {
        EndAction();
    }

    public virtual void DoAction()
    {
        //doNotDisplay = false;
        //OpenRightClick.doNotDisplay = true;

        CursorReplace.currentCursor = CursorType.Move;
        CursorReplace.SetCurrentCursorAsPrevious();
        waypointsList.Clear();
        targetList.Clear();
        unitInfos = Tools.Convert<TileInfo,UnitInfo>(MultiSelect.selectedTiles);
        MultiSelect.selectedTiles.Clear();
        MultiSelect.Relay();
        openRightClick.ResetValues();

        foreach (UnitInfo unit in unitInfos)
        {
            waypointsList.Add(unit.waypoints);
            targetList.Add(unit.targets);
        }

        ClearAllWaypoints();
        //actionDone = true;
        //Debug.Log("CLEARING XXXXX");

    }

    public virtual void EndAction()
    {
        Debug.Log("END ACTION");
        //tileInfoRaycaster.GetTileInfosFromPos(Input.mousePosition);
        CursorReplace.currentCursor = CursorType.Default;
        CursorReplace.SetCurrentCursorAsPrevious();
        unitInfos.Clear();
        //actionDone = false;
    }

    public void ClearAllWaypoints()
    {
        foreach (List<TileInfo> waypoints in waypointsList)
        {
            waypoints.Clear();
        }

        foreach (List<TileInfo> targets in targetList)
        {
            targets.Clear();
        }

        foreach (UnitInfo unit in unitInfos)
        {
            unit.currentTarget = null;
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
