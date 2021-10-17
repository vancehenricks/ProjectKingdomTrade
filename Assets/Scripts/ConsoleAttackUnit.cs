/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, October 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleAttackUnit : ConsoleCommand
{
    public UnitInfo unitInfo;
    public List<TileInfo> targetTiles;
    public bool log;

    protected override string SetCommand()
    {
        return "attack-unit";
    }

    public override void Initialize(Dictionary<string, string> subCommands)
    {
        subCommands.Add("log", "1");
        subCommands.Add("tile-id", "0");
        subCommands.Add("tile-object", "0");  
        subCommands.Add("target-tile-object", "1");  
        subCommands.Add("target-tile-id", "0|1");
        subCommands.Add("cancel", "");
        subCommands.Add("help", "");
    }

    private void ExecuteCommand()
    {
        unitInfo.targets.AddRange(targetTiles);
        unitInfo.unitEffect.unitWayPoint.UpdateWayPoint();   
        
        if(log)
        {
            ConsoleHandler.init.AddLine("Attacking unit");
            ConsoleHandler.init.AddCache(ConsoleHandler.init.previousCommand);
        }
    }

    public override void OnParsedConsoleEvent( Dictionary<string, string> subCommands, string[] arguments, params object[] objects)
    {
        unitInfo = null;
        log = true;
        targetTiles.Clear();

        foreach (string subCommand in subCommands.Keys)
        {
            //CDebug.Log(this, $"Command={subCommand}", LogType.Warning);
            switch (subCommand)
            {
                case "tile-id":
                    long tileId = 0;
                    long.TryParse(subCommands[subCommand], out tileId);
                    unitInfo = TileList.init.generatedUnits[tileId] as UnitInfo;
                    break;
                case "tile-object":
                    {
                        int index = 0;
                        int.TryParse(subCommands[subCommand], out index);
                        unitInfo = objects[index] as UnitInfo;
                    }
                    break;
                case "target-tile-id":
                    string[] ids = subCommands[subCommand].Split('|');
                    targetTiles.AddRange(ParseTileId<TileInfo>(ids));
                    break;
                case "target-tile-object":
                    {
                        int index = 0;
                        int.TryParse(subCommands[subCommand], out index);
                        targetTiles.AddRange(objects[index] as List<TileInfo>);
                    }
                    break;
                case "log":
                    if(subCommands[subCommand] == "0")
                    {
                        log = false;
                    }
                    break;
                case "cancel":
                    unitInfo.unitEffect.combatHandler.DisEngage();
                    unitInfo.merge = null;  
                    if(log)
                    {
                        ConsoleHandler.init.AddLine("move-unit for command cancelled");    
                        ConsoleHandler.init.AddCache("move-unit");                        
                    }
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
