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
    public delegate void OnSelectedChange(List<TileInfo> tiles);
    public static OnSelectedChange onSelectedChange;
    private static List<TileInfo> selectedTiles;
    public static bool shiftPressed;

    private void Start()
    {
        shiftPressed = false;
        selectedTiles = new List<TileInfo>();
        ExecuteCommands command = Command;
        CommandPipeline.Add(command, 200);
    }

	private void OnDestroy()
	{
		onSelectedChange = null;
	}

    private void Command()
    {
        if (InputOverride.GetKeyDown(KeyCode.LeftShift) ||
            InputOverride.GetKeyDown(KeyCode.RightShift))
        {
            shiftPressed = true;
        }

        if (InputOverride.GetKeyUp(KeyCode.LeftShift) ||
            InputOverride.GetKeyUp(KeyCode.RightShift))
        {
            shiftPressed = false;
        }
    }

    public static List<TileInfo> GetSelectedTiles()
    {
        return MultiSelect.selectedTiles;
    }

    public static void Add(TileInfo tileInfo, bool relay = false)
    {
        MultiSelect.selectedTiles.Add(tileInfo);
        if (!relay) return;
        onSelectedChange(MultiSelect.selectedTiles);
    }

    public static void AddRange(List<TileInfo> tileInfos, bool relay = false)
    {
        MultiSelect.selectedTiles.AddRange(tileInfos);
        if (!relay) return;
        onSelectedChange(MultiSelect.selectedTiles);
    }

    public static void Clear(bool relay = false)
    {
        MultiSelect.selectedTiles.Clear();
        if (!relay) return;
        onSelectedChange(MultiSelect.selectedTiles);
    }

    public static void Relay()
    {
        if (onSelectedChange == null) return;
        onSelectedChange(MultiSelect.selectedTiles);
    }
}
