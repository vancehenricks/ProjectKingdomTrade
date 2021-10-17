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


public class MoveUnit : PlayerCommand
{
    protected readonly string moveUnitCommand = "move-unit log:0 tile-object:0 target-tile-object:1";
    protected readonly string cancelMoveUnitCommand = "move-unit log:0 tile-object:0 cancel";

    protected override void Start()
    {
        base.Start();
        DragHandler.init.overrideOnBeginDrag += OverrideOnBeginDrag;
    }

    protected override void Command()
    {

        if (unitInfos.Count == 0) return;

        if (Input.GetButtonDown("Fire1") && MultiSelect.init.shiftPressed)
        {
            OpenLeftClick.init.Ignore();
            CDebug.Log(this,"Creating multiple waypoint...");
            TileInfo tileInfo = TileInfoRaycaster.init.GetTileInfoFromPos(Input.mousePosition);

            if(tileInfo != null)
            {
                List<TileInfo> tmp = new List<TileInfo>();
                Execute(Normalize(tileInfo), moveUnitCommand);
            }
        }

        if (Input.GetButtonDown("Fire1") && !MultiSelect.init.shiftPressed)
        {
            OpenLeftClick.init.Ignore();
            CDebug.Log(this,"Creating one waypoint... unitInfos.Count=" + unitInfos.Count);
            TileInfo tileInfo = TileInfoRaycaster.init.GetTileInfoFromPos(Input.mousePosition);
            ClearAllWaypoints();
            
            if(tileInfo != null)
            {
                Execute(Normalize(tileInfo), moveUnitCommand);
            }
        }

        if (Input.GetButtonDown("Fire2"))
        {
            OpenRightClick.init.doNotDisplay = true;
            OpenRightClick.init.skipRaycast = true;
            EndAction();
        }
    }

    public virtual void ClearAllWaypoints()
    {
        for (int i=0;i < unitInfos.Count;i++)
        {
            ConsoleParser.init.ConsoleEvent(cancelMoveUnitCommand, unitInfos[i]);
        }
    }

    private TileInfo Normalize(TileInfo tile)
    {
        TileInfo normalizedTile = tile;

        if(tile.tileType == "Unit")
        {
            UnitInfo unitInfo = tile as UnitInfo;

            if(unitInfo != null)
            {
                normalizedTile = unitInfo.standingTile;
            }
        }

        return normalizedTile;
    }

    private void OverrideOnBeginDrag(PointerEventData eventData)
    {
        EndAction();
    }

    public override void DoAction()
    {
        CursorReplace.init.currentCursor = CursorType.Move;

        base.DoAction();
        ClearAllWaypoints();
        OpenLeftClick.init.Ignore();
    }

}
