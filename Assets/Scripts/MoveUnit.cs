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
            openRightClick.openLeftClick.Ignore();
            CDebug.Log(this,"Creating multiple waypoint...");
            AssignsToList(Normalize(tileInfoRaycaster.GetTileInfoFromPos(Input.mousePosition)), waypointsList);
        }

        if (Input.GetButtonDown("Fire1") && !MultiSelect.init.shiftPressed)
        {
            openRightClick.openLeftClick.Ignore();
            CDebug.Log(this,"Creating one waypoint... unitInfos.Count=" + unitInfos.Count);
            TileInfo tileInfo = tileInfoRaycaster.GetTileInfoFromPos(Input.mousePosition);
            ClearAllWaypoints();
            AssignsToList(Normalize(tileInfo), waypointsList);
        }

        if (Input.GetButtonDown("Fire2"))
        {
            openRightClick.doNotDisplay = true;
            EndAction();
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

        openRightClick.openLeftClick.Ignore();
    }

}
