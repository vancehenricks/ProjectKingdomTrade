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
    public List<TileInfo> include;

    private List<TileInfo> _selectedTiles;
    private bool onSelectChange;

    protected override void Start()
    {
        base.Start();
        MultiSelect.onSelectedChange += OnSelectedChangeProxy;
        ExecuteCommands command = OnSelectedChange;
        CommandPipeline.Add(command, 500);
    }

    public override void DoAction()
    {
        base.DoAction();
        CursorReplace.currentCursor = CursorType.Attack;
        CursorReplace.SetCurrentCursorAsPrevious();
    }

    private void OnSelectedChange()
    {
        if (!onSelectChange) return;
        onSelectChange = false;

        if (unitInfos.Count == 0) return;

        if (_selectedTiles.Count > 0 && targetList.Count > 0)
        {
            openRightClick.openLeftClick.ignore = true;

            if (MultiSelect.shiftPressed)
            {
                //this assumes every data contain in selectedTiles are just duplicate
                //make sure to check if DoAction calls ClearAllWaypoints or this wont be true anymore
                List<TileInfo> distinctList = Tools.MergeList(_selectedTiles, targetList[0]);
                List<TileInfo> whiteListed = Tools.WhiteList(distinctList, include);

                AssignsToList(whiteListed, targetList);
            }
            else
            {
                List<TileInfo> sanitizeList = Tools.WhiteList(_selectedTiles, include);

                if (sanitizeList.Count == 0) return;

                ClearAllWaypoints();
                AssignsToList(sanitizeList[0], targetList);
            }
        }
    }

    protected void OnSelectedChangeProxy(List<TileInfo> selectedTiles)
    {
        onSelectChange = true;
        _selectedTiles = selectedTiles;
    }

    protected override void Command()
    {
        if (unitInfos.Count == 0) return;

        if (Input.GetButtonDown("Fire2"))
        {
            //ClearAllWaypoints();
            openRightClick.doNotDisplay = true;
            //Debug.Log("AttackUnit set doNotDisplay" + openRightClick.doNotDisplay);
            openRightClick.ResetValues();
            EndAction();
        }

        if (Input.GetButtonDown("Fire1"))
        {
            List<TileInfo> _tileInfos = new List<TileInfo>();
            tileInfoRaycaster.GetTileInfosFromPos(Input.mousePosition, _tileInfos);

            List<TileInfo> tileInfos = Tools.WhiteList(_tileInfos, include);

            Debug.Log("DEBUG-----" + tileInfos.Count);

            if (tileInfos.Count == 0) return;

            openRightClick.openLeftClick.ignore = true;

            if (tileInfos.Count == 1)
            {

                if (!MultiSelect.shiftPressed)
                {
                    ClearAllWaypoints();
                }

                AssignsToList(tileInfos[0], targetList);
                return;
            }

            openRightClick.include.AddRange(include);
            openRightClick.showOptions = false;
            openRightClick.whiteList = true;
            openRightClick.UseDefaultCursor = false;
            //openRightClick.multiSelect = false;
            openRightClick.tileInfoRaycaster.GetTileInfosFromPos(Input.mousePosition);
            openRightClick.forceDisplay = true;
            Debug.Log("SELECTED");

            //TileInfo tileInfo = tileInfoRaycaster.GetUnitInfoFromPos(Input.mousePosition);

            //ClearAllWaypoints();
            //AssignWaypointsToList(tileInfo);
        }

    }

    private IEnumerator closeCellDelay()
    {
        yield return new WaitForSeconds(0.4f);

        openRightClick.ResetValues();

        yield return null;
    }

    public override void EndAction()
    {
        base.EndAction();
    }
}