/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, November 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiSelect : MonoBehaviour
{
    private static MultiSelect _init;

    public static MultiSelect init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public System.Action<HashSet<TileInfo>> onSelectedChange;
    public HashSet<TileInfo> selectedTiles;
    public HashSet<TileInfo> previousSelectedTiles;
    public bool shiftPressed;
    public bool ctrlPressed;

    private void Awake()
    {
        selectedTiles = new HashSet<TileInfo>();
        previousSelectedTiles = new HashSet<TileInfo>();
        init = this;
    }

    private void Start()
    {
        CommandPipeline.init.Add(Command, 30);
    }

	private void OnDestroy()
	{
		onSelectedChange = null;
	}

    private void Command()
    {
        if (InputOverride.init.GetKeyDown(KeyCode.LeftShift) ||
            InputOverride.init.GetKeyDown(KeyCode.RightShift))
        {
            shiftPressed = true;
        }

        if (InputOverride.init.GetKeyUp(KeyCode.LeftShift) ||
            InputOverride.init.GetKeyUp(KeyCode.RightShift))
        {
            shiftPressed = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) || 
            Input.GetKeyDown(KeyCode.RightControl))
        {
            ctrlPressed = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl) || 
            Input.GetKeyUp(KeyCode.RightControl))
        {
            ctrlPressed = false;
        }  
    }

    public void Add(TileInfo tileInfo, bool relay = false)
    {
        selectedTiles.Add(tileInfo);
        if (!relay) return;
        Relay();
    }

    public void UnionWith(HashSet<TileInfo> tileInfos, bool relay = false)
    {
        selectedTiles.UnionWith(tileInfos);
        if (!relay) return;
        Relay();
    }

    public void Clear(bool relay = false)
    {
        previousSelectedTiles.Clear();
        previousSelectedTiles.UnionWith(selectedTiles);
        selectedTiles.Clear();
        if (!relay) return;
        Relay();
    }

    public void Relay()
    {
        if (onSelectedChange == null) return;
        onSelectedChange(selectedTiles);
    }
}
