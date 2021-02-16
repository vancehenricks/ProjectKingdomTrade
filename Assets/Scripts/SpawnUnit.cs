﻿/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, April 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpawnUnit : ConsoleCommand
{
    public List<UnitInfo> baseUnits;
    public Dictionary<string, UnitInfo> _baseUnits;
    public TileInfoRaycaster tileInfoRaycaster;
    //public KeyCode spawnKey;
    public int nCount;
    public int nMax;
    public bool autoFocus;
    public Color color;
    public int attackDistance;
    public bool executeAll;
    public bool notCancel;
    public int units;
    public string subType;
    public long playerId;
    public PlayerInfo playerInfo;
    public bool noMouseRequired;
    public Vector2 tileLocation;

    private bool fire1Clicked;

    public override void Initialize()
    {
        subCommands.Add("player-id", "0");
        subCommands.Add("sub-type", "Worker");
        subCommands.Add("amount", "1");
        subCommands.Add("tile-location", "0,0");
        subCommands.Add("color", "#ffffff");
        subCommands.Add("attack-distance", "1");
        subCommands.Add("units", "10");
        subCommands.Add("auto-focus", "true");
        subCommands.Add("execute-all", "");
        subCommands.Add("cancel", "");
        subCommands.Add("help", "");

        ConsoleHandler.init.AddCommand("spawn-unit", subCommands);
        ConsoleHandler.init.AddCache("spawn-unit");

        _baseUnits = new Dictionary<string, UnitInfo>();
        foreach (UnitInfo unit in baseUnits)
        {
            _baseUnits.Add(unit.subType, unit);
        }
    }

    private IEnumerator CommandStream()
    {
        while (nCount < nMax)
        {
            if (Input.GetButtonDown("Fire1") || (fire1Clicked && executeAll))
            {
                fire1Clicked = true;
                //make the units random color
                TileInfo tile = tileInfoRaycaster.GetTileInfoFromPos(Input.mousePosition);
                if (tile != null)
                {
                    Spawn(tile.transform.position);
                }
            }

            yield return null;
        }
    }

    private void ExecuteCommand()
    {
        Spawn(TileList.generatedTiles[tileLocation].transform.position);
    }

    public override void OnParsedConsoleEvent(string command, string[] arguments)
    {
        if (command == "spawn-unit")
        {
            Dictionary<string, string> subCommands = ConsoleParser.ArgumentsToSubCommands(arguments);
            SetDefaults();

            foreach (string subCommand in subCommands.Keys)
            {
                Debug.Log(subCommand);

                switch (subCommand)
                {
                    case "tile-location":
                        tileLocation = Tools.ParseLocation(subCommands[subCommand]);
                        noMouseRequired = true;
                        break;
                    case "player-id":
                        if (!PlayerList.init.players.ContainsKey(playerId)) break;

                        long.TryParse(subCommands[subCommand], out playerId);
                        playerInfo = PlayerList.init.players[playerId];
                        break;
                    case "sub-type":
                        if (!_baseUnits.ContainsKey(subCommands[subCommand])) break;
                        subType = subCommands[subCommand];
                        UnitInfo unitInfo = _baseUnits[subType].GetComponent<UnitInfo>();

                        if (!subCommands.ContainsKey("units"))
                        {
                            units = unitInfo.units;
                        }

                        if (!subCommands.ContainsKey("attackDistance"))
                        {
                            attackDistance = unitInfo.attackDistance;
                        }
                        break;
                    case "amount":
                        int.TryParse(subCommands[subCommand], out nMax);
                        break;
                    case "auto-focus":
                        bool.TryParse(subCommands[subCommand], out autoFocus);
                        break;
                    case "color":
                        ColorUtility.TryParseHtmlString(subCommands[subCommand], out color);
                        break;
                    case "attack-distance":
                        int.TryParse(subCommands[subCommand], out attackDistance);
                        break;
                    case "execute-all":
                        executeAll = true;
                        break;
                    case "units":
                        int.TryParse(subCommands[subCommand], out units);
                        break;
                    case "cancel":
                        nMax = 0;
                        notCancel = false;
                        ConsoleHandler.init.AddLine("spawn-unit command cancelled");
                        break;
                    case "help":
                    default:
                        nMax = 0;
                        notCancel = false;
                        ConsoleHandler.init.DisplaySubCommands("spawn-unit");
                        break;
                }
            }

            if (notCancel)
            {
                if (noMouseRequired)
                {
                    ExecuteCommand();
                }
                else
                {
                    ConsoleHandler.init.AddLine("Click a tile to spawn.");
                    StartCoroutine(CommandStream());
                }
            }
        }
    }

    private void Spawn(Vector3 loc)
    {
        GameObject unit = Instantiate(_baseUnits[subType].gameObject, _baseUnits[subType].transform.parent);
        unit.transform.position = loc;
        UnitInfo unitInfo = unit.GetComponent<UnitInfo>();
        //unitInfo.tileId = Tools.UniqueId + "";
        unitInfo.playerInfo = playerInfo;
        unitInfo.playerInfo.color = color == Color.white ? playerInfo.color : color;
        unitInfo.attackDistance = attackDistance;
        unitInfo.units = units;
        unitInfo.Initialize();
        unit.SetActive(true);
        ConsoleHandler.init.AddLine(string.Format("Spawning unit [{0}/{1}].", nCount + 1, nMax));

        if (autoFocus)
        {
            ConsoleHandler.init.Focus();
        }
        nCount++;

        if (nCount == 1)
        {
            ConsoleHandler.init.AddCache(ConsoleHandler.init.previousCommand);
        }
    }

    private void SetDefaults()
    {
        tileLocation = Vector2.zero;
        subType = "Worker";
        UnitInfo unitInfo = _baseUnits[subType].GetComponent<UnitInfo>();
        playerInfo = PlayerList.init.players.Values.ElementAt(0);
        playerId = playerInfo.playerId;
        attackDistance = unitInfo.attackDistance;
        units = unitInfo.units;
        nMax = 1;
        nCount = 0;
        autoFocus = true;
        color = Color.white;
        notCancel = true;
        executeAll = false;
        fire1Clicked = false;
        StopAllCoroutines();
    }
}