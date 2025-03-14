﻿/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, October 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleMergeUnit : ConsoleCommand
{
    public List<TileInfo> unitInfos;
    public bool log;

    protected override string SetCommand()
    {
        return "merge-unit";
    }

    public override void Initialize(Dictionary<string, string> subCommands)
    {
        string examples = 
        command + " tile-id:0|1 < unit 1 will merge with unit 0\n" +
        command + " tile-id:0|1|2 < unit 1 and unit 2 will merge with unit 0\n" +
        command + " tile-id:0|1 cancel\n" +
        command + " log:0 tile-id:0|1";

        subCommands.Add("*description","Merge unit to another unit with the same sub-type");
        subCommands.Add("*examples", examples);

        subCommands.Add("log", "0 or 1 default 1");
        subCommands.Add("tile-id", "0|1");
        subCommands.Add("tile-object", "0"); 
        subCommands.Add("cancel", "");
        subCommands.Add("help", "");
    }

    private void ExecuteCommand()
    {
        UnitInfo mergeUnit = unitInfos[0] as UnitInfo;
        
        if(unitInfos.Count  == 1)
        {
            Tools.Split(unitInfos[0] as UnitInfo);
        }
        else
        {
            for(int i = 1; i < unitInfos.Count;i++)
            {
                UnitInfo unit = unitInfos[i] as UnitInfo;
                unit.merge = mergeUnit;
                unit.unitEffect.mergeHandler.GenerateWayPoint();
                unit.unitEffect.unitWayPoint.UpdateWayPoint();            
            }
            
            if(log)
            {
                ConsoleHandler.init.AddLine("Merging unit");
                ConsoleHandler.init.AddCache(ConsoleHandler.init.previousCommand);
            }
        }
    }

    public override void OnParsedConsoleEvent( Dictionary<string, string> subCommands, string[] arguments, params object[] objects)
    {
        log = true;
        unitInfos.Clear();

        foreach (string subCommand in subCommands.Keys)
        {
            //CDebug.Log(this, $"Command={subCommand}", LogType.Warning);
            switch (subCommand)
            {
                case "tile-id":
                    {
                        string[] ids = subCommands[subCommand].Split('|');
                        unitInfos.AddRange(ParseTileId<TileInfo>(ids, true));
                    }
                    break;
                case "tile-object":
                    {
                        int index = 0;
                        int.TryParse(subCommands[subCommand], out index);
                        unitInfos.AddRange(objects[index] as List<TileInfo>);
                    }
                    break;
                case "log":
                    if(subCommands[subCommand] == "0")
                    {
                        log = false;
                    }
                    break;
                case "cancel":
                    for(int i = 1; i < unitInfos.Count;i++)
                    {
                        UnitInfo unit = unitInfos[i] as UnitInfo;

                        unit.unitEffect.combatHandler.DisEngage();
                        unit.merge = null; 
                        unit.unitEffect.unitWayPoint.UpdateWayPoint();
                    }
                    
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
