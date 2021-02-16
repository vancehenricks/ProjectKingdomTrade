/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleLoadTextures : ConsoleCommand
{
    public override void Initialize()
    {
        subCommands.Add("cancel", "");
        subCommands.Add("help", "");
        ConsoleHandler.init.AddCommand("load-textures", subCommands);
        ConsoleHandler.init.AddCache("load-textures");
    }

    private void ExecuteCommand()
    {
        PreLoaderHandler.init.LoadTextures();
        ConsoleHandler.init.AddLine("Textures Loaded!");
        ConsoleHandler.init.AddCache(ConsoleHandler.init.previousCommand);
    }

    public override void OnParsedConsoleEvent(string command, string[] arguments)
    {
        if (command == "load-textures")
        {
            Dictionary<string, string> subCommands = ConsoleParser.ArgumentsToSubCommands(arguments);

            foreach (string subCommand in subCommands.Keys)
            {
                switch (subCommand)
                {
                    case "cancel":
                        break;
                    case "help":
                    default:
                        ConsoleHandler.init.DisplaySubCommands("load-textures");
                        break;
                }
            }

            ExecuteCommand();
        }
    }
}
