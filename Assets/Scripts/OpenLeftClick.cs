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
    private static OpenLeftClick _init;
    public static OpenLeftClick init
    {
        get { return _init; }
        private set { _init = value; }
    }

    private bool ignore;

    public bool Ignore()
    {
        ignore = true;

        return ignore;
    }

    private void Awake()
    {
        init = this;
    }

    private void Start()
    {
        CommandPipeline.init.Add(Command, 999);
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
        //CDebug.Log(this,"OpenLeftClick");

        if (ignore) return;

        if (Input.GetButtonDown("Fire1"))
        {
            TileInfoRaycaster.init.GetTileInfosFromPos(Input.mousePosition);
        }

        if (Input.GetButtonUp("Fire1"))
        {
            List<RaycastResult> results = OpenRightClick.init.FireRayCast();

            if (results.Count > 0)
            {
                return;
            }

            int count = TileInfoRaycaster.init.tileInfos.Count;

            if (count > 1)
            {
                if (MultiSelect.init.shiftPressed)
                {                    
                    List<TileInfo> normalized = new List<TileInfo>();
                    foreach (TileInfo tile in TileInfoRaycaster.init.tileInfos)
                    {
                        if (tile.tileType == "Unit")
                        {
                            normalized.Add(tile);
                        }
                    }

                    TileInfoRaycaster.init.tileInfos.Clear();
                    TileInfoRaycaster.init.tileInfos.AddRange(MultiSelect.init.selectedTiles);
                    TileInfoRaycaster.init.tileInfos.AddRange(normalized);
                    MultiSelect.init.Clear(true);
                    MultiSelect.init.UnionWith(new HashSet<TileInfo>(TileInfoRaycaster.init.tileInfos));
                }

                MultiSelect.init.Clear(true);

                foreach (TileInfo tile in TileInfoRaycaster.init.tileInfos)
                {
                    if (tile.tileType != "Unit") continue;
                    MultiSelect.init.Add(tile);
                }

                MultiSelect.init.Relay();
                if(!MultiSelect.init.shiftPressed)
                {
                   MultiSelect.init.Clear(true); 
                }
            }
            else if (count == 1)
            {
                MultiSelect.init.Clear(true);
                MultiSelect.init.UnionWith(new HashSet<TileInfo>(TileInfoRaycaster.init.tileInfos), true);
            }
        }
    }

}
