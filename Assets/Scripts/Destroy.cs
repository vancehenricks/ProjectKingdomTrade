/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, April 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : ConsoleCommand
{
    public TileInfoRaycaster tileInfoRaycaster;
    //public bool isEnabled;
    public string type;
    public int nCount;
    public int nMax;
    public int targetLayer;
    public bool autoFocus;
    public bool executeAll;
    public long tileId;
    public bool noMouseRequired;

    private bool fire1Clicked;


    public override void Initialize()
    {
        Dictionary<string, string> subCommands = new Dictionary<string, string>();
        subCommands.Add("tile-id", "0");
        subCommands.Add("type", "Unit");
        subCommands.Add("amount", "1");
        subCommands.Add("layer", "1");
        subCommands.Add("auto-focus", "true");
        subCommands.Add("execute-all", "");
        subCommands.Add("cancel", "");
        subCommands.Add("help", "");

        ConsoleHandler.init.AddCommand("destroy", subCommands);
        ConsoleHandler.init.AddCache("destroy");
    }

    private IEnumerator CommandStream()
    {
        while (nCount < nMax)
        {
            if (Input.GetButtonDown("Fire1") || (fire1Clicked && executeAll))
            {
                fire1Clicked = true;
                //Debug.Log("test");

                //make the units random color
                List<TileInfo> tiles = new List<TileInfo>();
                tileInfoRaycaster.GetTileInfosFromPos(Input.mousePosition, tiles);

                int cLayer = 0;
                foreach (TileInfo tile in tiles)
                {
                    if (tile != null)
                    {

                        if (type == "Unit" && tile.tileType == type && cLayer == targetLayer)
                        {
                            UnitInfo unit = (UnitInfo)tile;
                            unit.Destroy();
                            AddLineAndCheckForFocus($"Destroyed tile [{nCount + 1}/{nMax}].");
                            break;
                        }
                        else if (type != "Unit" && tile.tileType == type && cLayer == targetLayer)
                        {
                            tile.Destroy();
                            AddLineAndCheckForFocus($"Destroyed unit [{nCount + 1}/{nMax}].");
                            break;
                        }
                        cLayer++;
                    }
                }
            }

            yield return null;
        }

        ConsoleHandler.init.AddLine("Done!");
    }

    private void AddLineAndCheckForFocus(string line)
    {
        ConsoleHandler.init.AddLine(string.Format(line));
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

    private void ExecuteCommand()
    {
        TileInfo tile = TileList.tileInfos[tileId];

        if (type == "Unit")
        {
            UnitInfo unit = (UnitInfo)tile;
            unit.Destroy();
            AddLineAndCheckForFocus($"Destroyed unit [{nCount + 1}/{nMax}].");
        }
        else if (type != "Unit")
        {
            tile.Destroy();
            AddLineAndCheckForFocus($"Destroyed tile [{nCount + 1}/{nMax}].");
        }
    }

    public override void OnParsedConsoleEvent(string command, string[] arguments)
    {
        if (command == "destroy")
        {
            Dictionary<string, string> subCommands = ConsoleParser.ArgumentsToSubCommands(arguments);

            tileId = 0;
            type = "Unit";
            nCount = 0;
            nMax = 1;
            targetLayer = 1;
            autoFocus = true;
            noMouseRequired = true;
            executeAll = false;
            fire1Clicked = false;
            StopAllCoroutines();

            foreach (string subCommand in subCommands.Keys)
            {
                switch (subCommand)
                {
                    case "type":
                        type = subCommands[subCommand];
                        break;
                    case "amount":
                        int.TryParse(subCommands[subCommand], out nMax);
                        break;
                    case "layer":
                        int.TryParse(subCommands[subCommand], out targetLayer);
                        break;
                    case "auto-focus":
                        bool.TryParse(subCommands[subCommand], out autoFocus);
                        break;
                    case "execute-all":
                        executeAll = true;
                        break;
                    case "tile-id":
                        long.TryParse(subCommands[subCommand], out tileId);
                        noMouseRequired = true;
                        break;
                    case "cancel":
                        ConsoleHandler.init.AddLine("destroy command cancelled");
                        return;
                    case "help":
                    default:
                        ConsoleHandler.init.DisplaySubCommands("destroy");
                        return;
                }
            }

            if (noMouseRequired)
            {
                ExecuteCommand();
            }
            else
            {
                ConsoleHandler.init.AddLine("Click a tile/unit to destroy...");
                StartCoroutine(CommandStream());
            }
        }
    }
}
