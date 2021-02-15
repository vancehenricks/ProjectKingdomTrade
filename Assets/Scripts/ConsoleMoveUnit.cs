/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleMoveUnit : ConsoleCommand
{
    public Vector2 tileLocation;
    public long tileId;

    public override void Initialize()
    {
        subCommands = new Dictionary<string, string>();
        subCommands.Add("tile-id", "0");
        subCommands.Add("tile-location","0,0");

        ConsoleHandler.init.AddCommand("move-unit", subCommands);
        ConsoleHandler.init.AddCache("move-unit");
    }

    private void ExecuteCommand()
    {

    }

    public override void OnParsedConsoleEvent(string command, string[] arguments)
    {
        if (command == "move-unit")
        {
            Dictionary<string, string> subCommands = ConsoleParser.ArgumentsToSubCommands(arguments);

            tileId = 0;
            tileLocation = Vector2.zero;

            foreach (string subCommand in subCommands.Keys)
            {
                Debug.Log(subCommand);

                switch (subCommand)
                {
                    case "tile-id":
                        long.TryParse(subCommands[subCommand], out tileId);
                        break;
                    case "tile-location":
                        tileLocation = Tools.ParseLocation(subCommands[subCommand]);
                        break;
                    case "cancel":
                        break;
                    case "help":
                    default:
                        ConsoleHandler.init.DisplaySubCommands("move-unit");
                        break;
                }
            }

            ExecuteCommand();
        }

    }
}
