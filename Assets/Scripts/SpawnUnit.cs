/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
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

    private bool fire1Clicked;

    public override void Initialize()
    {
        _baseUnits = new Dictionary<string, UnitInfo>();
        foreach (UnitInfo unit in baseUnits)
        {
            _baseUnits.Add(unit.subType, unit);
        }

        subCommands = new Dictionary<string, string>();
        subCommands.Add("player-id", "1");
        subCommands.Add("sub-type", "Worker");
        subCommands.Add("amount", "1");
        subCommands.Add("color", "#ffffff");
        subCommands.Add("attack-distance", "1");
        subCommands.Add("units", "10");
        subCommands.Add("auto-focus", "true");
        subCommands.Add("execute-all", "");
        subCommands.Add("cancel", "");
        subCommands.Add("help", "");

        ConsoleHandler.init.AddCommand("spawn-unit", subCommands);
        ConsoleHandler.init.AddCache("spawn-unit");
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
                    GameObject unit = Instantiate(_baseUnits[subType].gameObject, _baseUnits[subType].transform.parent);
                    unit.transform.position = tile.transform.position;
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
            }

            yield return null;
        }

        ConsoleHandler.init.AddLine("Done!");
    }

    public override void OnParsedConsoleEvent(string command, string[] arguments)
    {
        if (command == "spawn-unit")
        {
            Dictionary<string, string> subCommands = ConsoleParser.ArgumentsToSubCommands(arguments);
            SetParameters();

            foreach (string subCommand in subCommands.Keys)
            {
                Debug.Log(subCommand);

                switch (subCommand)
                {
                    case "player-id":
                        long.TryParse(subCommands[subCommand], out playerId);

                        if (!PlayerList.init.players.ContainsKey(playerId)) break;
                        playerInfo = PlayerList.init.players[playerId];
                        break;
                    case "sub-type":
                        if (!_baseUnits.ContainsKey(subCommands[subCommand])) break;
                        SetParameters(subType, playerInfo);
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
                ConsoleHandler.init.AddLine("Click a tile to spawn.");
                StartCoroutine(CommandStream());
            }
        }
    }

    private void SetParameters(string _subType = "Worker")
    {
        SetParameters(_subType, PlayerList.init.players.Values.ElementAt(0));
    }

    private void SetParameters(string _subType, PlayerInfo _playerInfo)
    {
        subType = _subType;
        UnitInfo unitInfo = _baseUnits[subType].GetComponent<UnitInfo>();

        playerInfo = _playerInfo;
        playerId = _playerInfo.playerId;
        nMax = 1;
        nCount = 0;
        autoFocus = true;
        color = Color.white;
        attackDistance = unitInfo.attackDistance;
        units = unitInfo.units;
        notCancel = true;
        executeAll = false;
        fire1Clicked = false;
        StopAllCoroutines();
    }
}