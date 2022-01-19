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
    //public KeyCode spawnKey;
    public int nCount;
    public int nMax;
    public bool autoFocus;
    public Color color;
    public int attackDistance;
    public bool executeAll;
    public int units;
    public string subType;
    public long playerId;
    public PlayerInfo playerInfo;
    public bool noMouseRequired;
    public TileInfo tileInfo;

    private bool fire1Clicked;

    private Coroutine commandStream;

    protected override string SetCommand()
    {
        return "spawn-unit";
    }

    public override void Initialize(Dictionary<string, string> subCommands)
    {
        string examples = 
        command + " TODO...\n";

        subCommands.Add("*description","Spawn unit in the selected tile");
        subCommands.Add("*examples", examples);

        subCommands.Add("player-id", "0");
        subCommands.Add("player-object", "0");
        subCommands.Add("sub-type", "Worker");
        subCommands.Add("amount", "1");
        subCommands.Add("tile-location", "0,0");
        subCommands.Add("tile-object", "0");
        subCommands.Add("color", "#ffffff");
        subCommands.Add("attack-distance", "1");
        subCommands.Add("units", "10");
        subCommands.Add("auto-focus", "0 or 1 default 1");
        subCommands.Add("execute-all", "");
        subCommands.Add("cancel", "");
        subCommands.Add("help", "");
    }

    private IEnumerator CommandStream()
    {
        
        while (nCount < nMax)
        {
            if (Input.GetButtonDown("Fire1") || (fire1Clicked && executeAll))
            {
                fire1Clicked = true;
                //make the units random color
                TileInfo tile = TileInfoRaycaster.init.GetTileInfoFromPos(Input.mousePosition);
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
        Spawn(tileInfo.transform.position);
    }

    public override void OnParsedConsoleEvent(Dictionary<string, string> subCommands, string[] arguments, params object[] objects)
    {
        SetDefaults();

        foreach (string subCommand in subCommands.Keys)
        {
            //CDebug.Log(subCommand);
            int index = 0;

            switch (subCommand)
            {
                case "tile-object":
                    int.TryParse(subCommands[subCommand], out index);
                    tileInfo = objects[index] as TileInfo;
                break;
                case "player-object":
                    int.TryParse(subCommands[subCommand], out index);
                    playerInfo = objects[index] as PlayerInfo;
                break;
                case "tile-location":
                    Vector2Int tileLocation = Tools.ParseLocation(subCommands[subCommand]);
                    tileInfo = TileList.init.generatedTiles[tileLocation];
                    noMouseRequired = true;
                    break;
                case "player-id":
                    if (!PlayerList.init.players.ContainsKey(playerId)) break;

                    long.TryParse(subCommands[subCommand], out playerId);
                    playerInfo = PlayerList.init.players[playerId];
                    break;
                case "sub-type":
                    if (!TileConfigHandler.init.baseUnits.ContainsKey(subCommands[subCommand])) break;
                    subType = subCommands[subCommand];
                    UnitInfo unitInfo = TileConfigHandler.init.baseUnits[subType].GetComponent<UnitInfo>();

                    if (!subCommands.ContainsKey("units"))
                    {
                        units = unitInfo.unit;
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
                    ConsoleHandler.init.AddLine(command + " command cancelled");
                    return;
                case "help":
                default:
                    ConsoleHandler.init.DisplaySubCommands(command);
                    return;
            }
        }

        if (noMouseRequired)
        {
            ExecuteCommand();
        }
        else
        {
            ConsoleHandler.init.AddLine("Click a tile to spawn.");
            commandStream = StartCoroutine(CommandStream());
        }
    }

    private void Spawn(Vector3 loc)
    {
        GameObject baseTile = TileConfigHandler.init.baseUnits[subType].gameObject;
        GameObject unit = Instantiate(TileConfigHandler.init.baseUnits[subType].gameObject, TileList.init.subGrids[Vector3Int.FloorToInt(loc)]);
        unit.name = baseTile.name;
        unit.transform.position = loc;
        UnitInfo unitInfo = unit.GetComponent<UnitInfo>();
        //unitInfo.tileId = Tools.UniqueId + "";
        unitInfo.playerInfo = playerInfo;
        unitInfo.playerInfo.color = color == Color.white ? playerInfo.color : color;
        unitInfo.attackDistance = attackDistance;
        unitInfo.unit = units;
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
        subType = "Worker";
        UnitInfo unitInfo = TileConfigHandler.init.baseUnits[subType].GetComponent<UnitInfo>();
        playerInfo = PlayerList.init.players.Values.ElementAt(0);
        playerId = playerInfo.playerId;
        attackDistance = unitInfo.attackDistance;
        units = unitInfo.unit;
        nMax = 1;
        nCount = 0;
        autoFocus = true;
        color = Color.white;
        executeAll = false;
        fire1Clicked = false;
        tileInfo = null;

        if (commandStream != null)
        {
            StopCoroutine(commandStream);
        }
    }
}