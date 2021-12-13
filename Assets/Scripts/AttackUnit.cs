/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, June 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;


public class AttackUnit : MoveUnit
{
    protected readonly string attackUnitCommand = "attack-unit log:0 tile-object:0 target-tile-object:1";

    public List<TileInfo> include;

    private HashSet<TileInfo> _selectedTiles;
    private bool onSelectChange;

    protected override void Start()
    {
        base.Start();
        _selectedTiles = new HashSet<TileInfo>();
        MultiSelect.init.onSelectedChange += OnSelectedChangeProxy;
        ExecuteCommands command = OnSelectedChange;
        CommandPipeline.init.Add(command, 500);
    }

    private void OnDestroy()
    {
        MultiSelect.init.onSelectedChange -= OnSelectedChangeProxy;
    }

    public override void DoAction()
    {
        base.DoAction();
        CursorReplace.init.currentCursor = CursorType.Attack;
    }

    private void OnSelectedChange()
    {
        if (!onSelectChange) return;
        onSelectChange = false;

        if (unitInfos.Count == 0) return;

        if (_selectedTiles.Count > 0 /*&& targetList.Count > 0*/)
        {
            OpenLeftClick.init.Ignore();
            List<TileInfo> whiteListed = Tools.WhiteListTileType(new List<TileInfo>(_selectedTiles), include);
            
            if (MultiSelect.init.shiftPressed)
            {
                Execute(whiteListed, attackUnitCommand);
            }
            else
            {
                if (whiteListed.Count == 0) return;
                ClearAllWaypoints();
                Execute(whiteListed[0], attackUnitCommand);
            }
        }
    }

    protected void OnSelectedChangeProxy(HashSet<TileInfo> selectedTiles)
    {
        onSelectChange = true;
        _selectedTiles = selectedTiles;
    }

    protected override void Command()
    {
        if (unitInfos.Count == 0) return;

        if (Input.GetButtonDown("Fire2"))
        {
            OpenRightClick.init.doNotDisplay = true;
            OpenRightClick.init.skipRaycast = true;
            CDebug.Log(this,"AttackUnit set doNotDisplay" + OpenRightClick.init.doNotDisplay);
            OpenRightClick.init.ResetValues();
            EndAction();
        }

        if (Input.GetButtonDown("Fire1"))
        {
            List<TileInfo> _tileInfos = new List<TileInfo>();
            TileInfoRaycaster.init.GetTileInfosFromPos(Input.mousePosition, _tileInfos);

            List<TileInfo> tileInfos = Tools.WhiteListTileType(_tileInfos, include);

            CDebug.Log(this,"TileInfo.Count=" + tileInfos.Count);

            if (tileInfos.Count == 0) return;

            OpenLeftClick.init.Ignore();

            if (tileInfos.Count == 1)
            {

                if (!MultiSelect.init.shiftPressed)
                {
                    ClearAllWaypoints();
                }

                Execute(tileInfos[0], attackUnitCommand);
                return;
            }

            OpenRightClick.init.include.AddRange(include);
            OpenRightClick.init.showOptions = false;
            OpenRightClick.init.whiteList = true;
            OpenRightClick.init.useDefaultCursor = false;
            //openRightClick.init.multiSelect = false;
            TileInfoRaycaster.init.GetTileInfosFromPos(Input.mousePosition);
            OpenRightClick.init.forceDisplay = true;
            CDebug.Log(this, "Unit Selected!");
        }

    }

    private IEnumerator CloseCellDelay()
    {
        yield return new WaitForSeconds(0.4f);

        OpenRightClick.init.ResetValues();

        yield return null;
    }
}