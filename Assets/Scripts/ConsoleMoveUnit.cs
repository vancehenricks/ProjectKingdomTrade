﻿/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2021
 */

using DebugHandler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleMoveUnit : ConsoleCommand
{
    public Vector2Int tileLocation;
    public long tileId;

    public override void Initialize()
    {
        Dictionary<string, string> subCommands = new Dictionary<string, string>();
        subCommands.Add("tile-id", "0");
        subCommands.Add("tile-location","0,0");

        ConsoleHandler.init.AddCommand("move-unit", subCommands);
        ConsoleHandler.init.AddCache("move-unit");
    }

    private void ExecuteCommand()
    {
        UnitInfo unitInfo = (UnitInfo)TileList.init.generatedUnits[tileId];
        unitInfo.unitEffect.combatHandler.DisEngage();
        unitInfo.merge = null;
        unitInfo.waypoints.Add(TileList.init.generatedTiles[tileLocation]);

        ConsoleHandler.init.AddLine("Moving unit " + tileId + " to tile " + tileLocation);
        ConsoleHandler.init.AddCache(ConsoleHandler.init.previousCommand);
    }

    public override void OnParsedConsoleEvent(string command, string[] arguments)
    {
        if (command == "move-unit")
        {
            Dictionary<string, string> subCommands = ConsoleParser.init.ArgumentsToSubCommands(arguments);

            tileId = 0;
            tileLocation = Vector2Int.zero;

            foreach (string subCommand in subCommands.Keys)
            {
                CDebug.Log(this, $"Command={subCommand}", LogType.Warning);

                switch (subCommand)
                {
                    case "tile-id":
                        long.TryParse(subCommands[subCommand], out tileId);
                        break;
                    case "tile-location":
                        tileLocation = Tools.ParseLocation(subCommands[subCommand]);
                        break;
                    case "cancel":
                        return;
                    case "help":
                    default:
                        ConsoleHandler.init.DisplaySubCommands("move-unit");
                        return;
                }
            }

            ExecuteCommand();
        }

    }
}
