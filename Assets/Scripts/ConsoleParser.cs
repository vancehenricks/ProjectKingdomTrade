﻿/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, April 2020
 */

using System;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleParser : MonoBehaviour
{
    public delegate void OnParsedConsoleEvent(string command, string[] arguments);
    public static OnParsedConsoleEvent onParsedConsoleEvent;

    private void Start()
    {
        ConsoleHandler.onConsoleEvent += OnConsoleEvent;
    }

    //spawn_unit type:archer amount:1000 loc:mousePosition

    private void OnConsoleEvent(string rawCommands)
    {
        string[] commandArray = rawCommands.Split(' ');
        string command = commandArray[0];
        string[] arguments = new string[commandArray.Length - 1];
        Array.Copy(commandArray, 1, arguments, 0, arguments.Length);

        onParsedConsoleEvent(command, arguments);
    }

    public static Dictionary<string, string> ArgumentsToSubCommands(string[] arguments)
    {
        Dictionary<string, string> subCommands = new Dictionary<string, string>();

        foreach (string argument in arguments)
        {
            string[] subCommand = argument.Split(':');
            if (subCommand.Length > 1)
            {
                subCommands.Add(subCommand[0], subCommand[1]);
            }
            else
            {
                subCommands.Add(subCommand[0], "");
            }
        }

        return subCommands;
    }

}
