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
    public static List<TileInfo> selectedTiles;
    public static bool shiftPressed;

    private void Start()
    {
        selectedTiles = new List<TileInfo>();
        ExecuteCommands command = Command;
        CommandPipeline.Add(command, 200);
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

    public static void Relay()
    {
        if (onSelectedChange == null) return;
        onSelectedChange(MultiSelect.selectedTiles);
    }
}
