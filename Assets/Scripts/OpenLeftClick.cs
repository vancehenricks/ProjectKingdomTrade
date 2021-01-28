/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, Febuary 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OpenLeftClick : MonoBehaviour
{
    public TileInfoRaycaster tileInfoRaycaster;
    public OpenRightClick openRightClick;

    private bool ignore;

    public bool Ignore()
    {
        ignore = true;

        return ignore;
    }

    private void Start()
    {
        ExecuteCommands command = Command;
        ExecuteCommands preCommand = PreCommand;

        CommandPipeline.Add(command, 1001);
        CommandPipeline.Add(preCommand, 49);
    }

    private void PreCommand()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            ignore = false;
        }
    }

    private void Command()
    {
        if (ignore) return;

        if (Input.GetButtonDown("Fire1"))
        {
            tileInfoRaycaster.GetTileInfosFromPos(Input.mousePosition);
        }

        if (Input.GetButtonUp("Fire1"))
        {
            List<RaycastResult> results = openRightClick.FireRayCast();

            if (results.Count > 0)
            {
                return;
            }

            int count = TileInfoRaycaster.tileInfos.Count;

            if (count > 1)
            {
                MultiSelect.Clear(true);

                foreach (TileInfo tile in TileInfoRaycaster.tileInfos)
                {
                    if (tile.tileType != "Unit") continue;
                    MultiSelect.Add(tile);
                }

                MultiSelect.Relay();
            }
            else if (count == 1)
            {
                MultiSelect.Clear(true);
                MultiSelect.AddRange(TileInfoRaycaster.tileInfos, true);
            }
        }
    }

}
