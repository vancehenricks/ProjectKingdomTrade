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

    public delegate void OnSelectedChange(List<TileInfo> tiles);
    public OnSelectedChange onSelectedChange;
    public List<TileInfo> selectedTiles;
    public bool shiftPressed;

    private void Awake()
    {
        init = this;
    }

    private void Start()
    {
        shiftPressed = false;
        selectedTiles = new List<TileInfo>();
        CommandPipeline.init.Add(Command, 200);
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
    }

    public void Add(TileInfo tileInfo, bool relay = false)
    {
        selectedTiles.Add(tileInfo);
        if (!relay) return;
        Relay();
    }

    public void AddRange(List<TileInfo> tileInfos, bool relay = false)
    {
        selectedTiles.AddRange(tileInfos);
        if (!relay) return;
        Relay();
    }

    public void Clear(bool relay = false)
    {
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
