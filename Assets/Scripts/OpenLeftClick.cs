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
        CommandPipeline.init.Add(Command, 1001);
        CommandPipeline.init.Add(PreCommand, 49);
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

            int count = TileInfoRaycaster.init.tileInfos.Count;

            if (count > 1)
            {
                MultiSelect.init.Clear(true);

                foreach (TileInfo tile in TileInfoRaycaster.init.tileInfos)
                {
                    if (tile.tileType != "Unit") continue;
                    MultiSelect.init.Add(tile);
                }

                MultiSelect.init.Relay();
            }
            else if (count == 1)
            {
                MultiSelect.init.Clear(true);
                MultiSelect.init.AddRange(TileInfoRaycaster.init.tileInfos, true);
            }
        }
    }

}
