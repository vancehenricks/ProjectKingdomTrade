/* Copyright (C) 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, Febuary 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenLeftClick : MonoBehaviour
{
    public TileInfoRaycaster tileInfoRaycaster;

    private void Start ()
    {
        ExecuteCommands command = Command;
        CommandPipeline.Add(command, 900);
	}

    private void Command()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            tileInfoRaycaster.GetTileInfosFromPos(Input.mousePosition);
        }
    }
	
}
