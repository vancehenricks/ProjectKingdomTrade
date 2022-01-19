/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2021
 */


using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ConsoleMoveUnit : ConsoleCommand
{
    public UnitInfo unitInfo;
    public List<TileInfo> targetTiles;
    public bool log;

    protected override string SetCommand()
    {
        return "move-unit";
    }

    public override void Initialize(Dictionary<string, string> subCommands)
    {
        string examples = 
        command + " tile-id:0 target-tile-id:1\n" +
        command + " tile-id:0 target-tile-id:1|2|1\n" +
        command + " tile-id:0 target-tile-location:0,0\n" + 
        command + " tile-id:0 target-tile-location:0,0|0,1|0,2\n" + 
        command + " tile-id:0 cancel\n" +
        command + " log:0 tile-id:0 target-tile-id:1";

        subCommands.Add("*description","Move unit to designated tile");
        subCommands.Add("*examples", examples);

        subCommands.Add("log", "0 or 1 default 1");
        subCommands.Add("tile-id", "0");
        subCommands.Add("tile-object", "0");  
        subCommands.Add("target-tile-id", "0 or 0|1");
        subCommands.Add("target-tile-location", "0,0 or 0,0|0,1");
        subCommands.Add("target-tile-object", "1");  
        subCommands.Add("cancel", "");
        subCommands.Add("help", "");
    }

    private void ExecuteCommand()
    {
        unitInfo.waypoints.AddRange(targetTiles);
        unitInfo.unitEffect.unitWayPoint.UpdateWayPoint();
        //unitInfo.unitEffect.combatHandler.DisEngage();
        //unitInfo.merge = null;        
        
        if(log)
        {
            ConsoleHandler.init.AddLine("Moving unit");
            ConsoleHandler.init.AddCache(ConsoleHandler.init.previousCommand);
        }
        //}
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
                case "target-tile-location":
                    string[] locations = subCommands[subCommand].Split('|');
                    targetTiles.AddRange(ParseTileLocation<TileInfo>(locations));
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
                        ConsoleHandler.init.AddLine(command + " for command cancelled");    
                        ConsoleHandler.init.AddCache(command);                        
                    }
                    return;
                case "help":
                default:
                    ConsoleHandler.init.DisplaySubCommands(command);
                    return;
            }
        }

        ExecuteCommand();
    }
}
