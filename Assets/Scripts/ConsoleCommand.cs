/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, November 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConsoleCommand : MonoBehaviour
{
    private string _command;
    public string command
    {
        get
        {
            return _command;
        }

        private set
        {
            _command = value;
        }
    }

    private void Start()
    {
        ConsoleParser.init.onParsedConsoleEvent += PreParseConsoleEvent;
        command = SetCommand();
        Dictionary<string, string> subCommands = new Dictionary<string, string>();
        Initialize(subCommands);
        ConsoleHandler.init.AddCommand(command, subCommands);
        ConsoleHandler.init.AddCache(command);        
    }

    protected abstract string SetCommand();

    private void OnDestroy()
    {
        //if console object is not active at all it will not call OnDestroy on those thus not clearing the delegates
        ConsoleParser.init.onParsedConsoleEvent -= PreParseConsoleEvent;
    }

    public abstract void Initialize(Dictionary<string, string> subCommands);
    public abstract void OnParsedConsoleEvent(Dictionary<string, string> subCommands, string[] arguments, params object[] objects);

    private void PreParseConsoleEvent(string command, string[] arguments, params object[] objects)
    {
        if(command == this.command)
        {
            Dictionary<string, string> subCommands = ConsoleParser.init.ArgumentsToSubCommands(arguments);
            OnParsedConsoleEvent(subCommands, arguments, objects);
        }
    }

    /*public int IsVariable(string subCommand)
    {
        if(subCommand.Contains("*"))
        {
           string normalized = subCommand.Remove(0);
           int index = 0;

           int.TryParse(normalized, out index);

           return index;
        }

        return -1;
    }*/

    public List<T> ParseTileId <T> (string[] ids, bool findUnits = false) where T: TileInfo
    {
        List<T> finalTileInfo = new List<T>();

        foreach(string id in ids)
        {
            long normalizedId = 0;
            long.TryParse(id, out normalizedId);

            if(findUnits)
            {
                finalTileInfo.Add(TileList.init.generatedUnits[normalizedId] as T);
            }
            else
            {
                finalTileInfo.Add(TileList.init.tileInfos[normalizedId] as T);
            }

        }

        return finalTileInfo;
    }

    public List<T> ParseTileLocation <T> (string[] locations) where T: TileInfo
    {
        List<T> finalTileInfo = new List<T>();
        List<Vector2Int> normalizedLocations = Tools.ParseLocation(locations);

        foreach(Vector2Int location in normalizedLocations)
        {
            finalTileInfo.Add(TileList.init.generatedTiles[location] as T);
        }

        return finalTileInfo;
    }  
}