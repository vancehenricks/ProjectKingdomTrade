/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandTest : MonoBehaviour
{
    public KeyCode keyCode;

    private void Update()
    {
        if (Input.GetKeyDown(keyCode))
        {
            ConsoleParser.init.OnConsoleEvent("spawn-unit amount:3");
        }
    }
}
