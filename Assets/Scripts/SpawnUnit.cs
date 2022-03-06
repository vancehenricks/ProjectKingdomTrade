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
    public UnitInfo baseUnitInfo;
    public bool log;

    private bool fire1Clicked;

    private Coroutine commandStream;

    protected override string SetCommand()
    {
        return "spawn-unit";
    }

    public override void Initialize(Dictionary<string, string> subCommands)
    {
        string examples = 
        command + " player-id:0 amount:10 < spawn a Worker tile under player 0 for every left click on a tile\n" +
        command + " player-id:0 amount:10 execute-all < spawns all Worker tile in one go\n" +
        command + " player-id:0 sub-type:Infantry\n" +
        command + " player-id:0 amount:5 sub-type:Infantry tile-location:0,0\n" + 
            "\t< spawns 5 unit of Infantry in given tile-location\n" +
        command + " player-id:0 tile-id:0\n" +
        command + " player-id:0 units:20 attack-distance:2 sub-type:Infantry tile-id:0\n" +
            "\t< spawns an Infantry unit with modified properties\n";

        subCommands.Add("*description","Spawn unit in the selected tile");
        subCommands.Add("*examples", examples);

        subCommands.Add("log", "0 or 1 default 1");
        subCommands.Add("player-id", "0");
        subCommands.Add("player-object", "0");
        subCommands.Add("sub-type", "Worker");
        subCommands.Add("sub-type-object", "0");
        subCommands.Add("amount", "1");
        subCommands.Add("tile-location", "0,0");        
        subCommands.Add("tile-id", "0");
        subCommands.Add("tile-object", "0");
        subCommands.Add("color", "#ffffff");
        subCommands.Add("attack-distance", "default whatever sub-type is");
        subCommands.Add("units", "default whatever sub-type is");
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

                if(noMouseRequired)
                {
                    Spawn(tileInfo.transform.position);
                }
                else
                {
                    TileInfo tile = TileInfoRaycaster.init.GetTileInfoFromPos(Input.mousePosition);
                    if (tile != null)
                    {
                        Spawn(tile.transform.position);
                    }
                }
            }

            yield return null;
        }
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
                case "log":
                    if(subCommands[subCommand] == "0")
                    {
                        log = false;
                    }
                    break;
                case "tile-object":
                    int.TryParse(subCommands[subCommand], out index);
                    tileInfo = objects[index] as TileInfo;
                    noMouseRequired = true;
                break;
                case "player-object":
                    int.TryParse(subCommands[subCommand], out index);
                    playerInfo = objects[index] as PlayerInfo;
                break;
                case "tile-id":
                    long tileId = 0;
                    long.TryParse(subCommands[subCommand], out tileId);
                    tileInfo = TileList.init.tileInfos[tileId];
                    noMouseRequired = true;
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
                case "sub-type-object":
                    int.TryParse(subCommands[subCommand], out index);
                    baseUnitInfo = objects[index] as UnitInfo;
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
                    if(log)
                    {
                        ConsoleHandler.init.AddLine(command + " command cancelled");
                    }
                    return;
                case "help":
                default:
                    ConsoleHandler.init.DisplaySubCommands(command);
                    return;
            }
        }
        
        if(!noMouseRequired)
        {
            if(log)
            {
                ConsoleHandler.init.AddLine("Click a tile to spawn.");
            }
        }
        else
        {
            executeAll = true;
            fire1Clicked = true;
        }

        commandStream = StartCoroutine(CommandStream());
    }

    private void Spawn(Vector3 loc)
    {
        GameObject baseTile = baseUnitInfo == null ? TileConfigHandler.init.baseUnits[subType].gameObject : baseUnitInfo.gameObject;
        GameObject unit = Instantiate(baseTile, TileList.init.subGrids[Vector3Int.FloorToInt(loc)]);
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
        if(log)
        {
            ConsoleHandler.init.AddLine(string.Format("Spawning unit [{0}/{1}].", nCount + 1, nMax));
        }

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
        log = true;
        baseUnitInfo = null;
        noMouseRequired = false;

        if (commandStream != null)
        {
            StopCoroutine(commandStream);
        }
    }
}