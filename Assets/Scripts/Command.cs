/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, November 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command : MonoBehaviour
{
    public Dictionary<string, string> subCommands;

    private void Start()
    {
        ConsoleHandler.initialize += Initialize;
        ConsoleParser.onParsedConsoleEvent += OnParsedConsoleEvent;
    }

    public virtual void Initialize()
    {
        //implementation
    }

    public virtual void OnParsedConsoleEvent(string command, string[] arguments)
    {
        //implementation
    }
}