﻿/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, August 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MoveUnit : PlayerCommand
{

    public List<UnitInfo> unitInfos;
    public List<List<TileInfo>> waypointsList;
    public List<List<TileInfo>> targetList;

    protected override void Start()
    {
        base.Start();

        waypointsList = new List<List<TileInfo>>();
        targetList = new List<List<TileInfo>>();

        DragHandler.overrideOnBeginDrag += OverrideOnBeginDrag;
    }

    protected override void Command()
    {
        //Debug.Log("MoveUnit");

        base.Command();

        if (unitInfos.Count == 0) return;

        if (Input.GetButtonDown("Fire1") && MultiSelect.shiftPressed)
        {
            openRightClick.openLeftClick.Ignore();
            Debug.Log("Creating multiple waypoint...");
            AssignsToList(tileInfoRaycaster.GetTileInfoFromPos(Input.mousePosition), waypointsList);
            //shiftSelected = false;
        }

        if (Input.GetButtonDown("Fire1") && !MultiSelect.shiftPressed)
        {
            openRightClick.openLeftClick.Ignore();
            Debug.Log("Creating one waypoint... unitInfos.Count=" + unitInfos.Count);
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

    public override void DoAction()
    {
        //doNotDisplay = false;
        //OpenRightClick.doNotDisplay = true;
        base.DoAction();

        CursorReplace.currentCursor = CursorType.Move;
        CursorReplace.SetCurrentCursorAsPrevious();
        waypointsList.Clear();
        targetList.Clear();
        unitInfos = Tools.Convert<TileInfo, UnitInfo>(MultiSelect.GetSelectedTiles());
        MultiSelect.Clear(true);
        openRightClick.ResetValues();

        foreach (UnitInfo unit in unitInfos)
        {
            waypointsList.Add(unit.waypoints);
            targetList.Add(unit.targets);
        }

        ClearAllWaypoints();
        //actionDone = true;
        //Debug.Log("CLEARING XXXXX");
        openRightClick.openLeftClick.Ignore();
    }

    public override void EndAction()
    {
        base.EndAction();

        Debug.Log("END ACTION");
        //tileInfoRaycaster.GetTileInfosFromPos(Input.mousePosition);
        CursorReplace.currentCursor = CursorType.Default;
        CursorReplace.SetCurrentCursorAsPrevious();
        unitInfos.Clear();
        //actionDone = false;
    }

    public void ClearAllWaypoints()
    {

        foreach (List<TileInfo> target in targetList)
        {
            target.Clear();
        }

        foreach (UnitInfo unit in unitInfos)
        {
            unit.unitEffect.combatHandler.DisEngage();
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
