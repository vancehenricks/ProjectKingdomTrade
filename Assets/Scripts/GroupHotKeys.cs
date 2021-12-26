/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, September 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupHotKeys : MonoBehaviour
{
    private static GroupHotKeys _init;
    public static GroupHotKeys init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public Dictionary<KeyCode, List<TileInfo>> groups;
    private KeyCode previousKey;

    private void Awake()
    {
        init = this;
    }

    private void Start()
    {
        groups = new Dictionary<KeyCode, List<TileInfo>>();

        for(int i =(int)KeyCode.Alpha0;i <= (int)KeyCode.Alpha9;i++)
        {
            groups.Add((KeyCode)i, new List<TileInfo>());
        }

        CommandPipeline.init.Add(Command, 998);
        //CommandPipeline.init.Add(PreCommand, 48);
    }

    /*private void PreCommand()
    {
        if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2"))
        {
            previousKey = KeyCode.None;
        }
    }*/

    private void Command()
    {
        if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2"))
        {
            previousKey = KeyCode.None;
        }

        if (OpenRightClick.init.skipRaycast && Input.GetButtonDown("Fire2"))
        {
            TileInfoRaycaster.init.tileInfos.Clear();
            CDebug.Log(this, "MultiSelect.init.selectedTiles.Count="+MultiSelect.init.selectedTiles.Count, LogType.Warning);
            TileInfoRaycaster.init.tileInfos.AddRange(MultiSelect.init.previousSelectedTiles);
        }
        else if (MultiSelect.init.shiftPressed && Input.GetButtonDown("Fire2"))
        {
            TileInfoRaycaster.init.tileInfos.RemoveAll(tileInfo => tileInfo == null);
            OpenRightClick.init.skipRaycast = true;
        }

        for(int i =(int)KeyCode.Alpha0;i <= (int)KeyCode.Alpha9;i++)
        {
            KeyCode key = (KeyCode)i;

            if(Input.GetKeyDown(key))
            {
                if(MultiSelect.init.ctrlPressed)
                {
                    CDebug.Log(this, "Created group for " + key, LogType.Warning);
                    if(OpenRightClick.init.generateCells.gameObject.activeSelf)
                    {
                        groups[key] = new List<TileInfo>(MultiSelect.init.selectedTiles);
                    }
                    else
                    {
                        groups[key] = new List<TileInfo>(MultiSelect.init.previousSelectedTiles);
                    }
                }
                else if (groups[key].Count > 0)
                {
                    bool skip = false;
                    if(previousKey != key)
                    {
                        previousKey = key;
                        skip = true;
                    }
                    groups[key].RemoveAll(tileInfo => tileInfo == null);
                    CDebug.Log(this, "Retrieving group for " + key, LogType.Warning);
                    TileInfoRaycaster.init.tileInfos.Clear();
                    TileInfoRaycaster.init.tileInfos.AddRange(groups[key]);
                    MultiSelect.init.Clear(true);
                    MultiSelect.init.UnionWith(new HashSet<TileInfo>(groups[key]), true);
                    MultiSelect.init.Clear(true);

                    if(!skip && previousKey == key)
                    {
                        previousKey = KeyCode.None;
                        ResetCenter.init.DoAction();
                    }
                }
            }
        }
    }  
}
