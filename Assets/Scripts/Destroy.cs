/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, April 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    public TileInfoRaycaster tileInfoRaycaster;
    //public bool isEnabled;
    public string type;
    public int nCount;
    public int nMax;
    public int targetLayer;
    public bool autoFocus;

    private void Start()
    {
        ConsoleParser.onParsedConsoleEvent += OnParsedConsoleEvent;
    }

    private void Update()
    {
        if (nCount < nMax && Input.GetButtonDown("Fire1"))
        {
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
                        unit.End();
                        AddLineAndCheckForFocus(string.Format("Destroyed unit [{0}/{1}].", nCount + 1, nMax));
                        break;
                    }
                    else if (type != "Unit" && tile.tileType == type && cLayer == targetLayer)
                    {
                        tile.End();
                        AddLineAndCheckForFocus(string.Format("Destroyed tile [{0}/{1}].", nCount + 1, nMax));
                        break;
                    }
                    cLayer++;
                }
            }
        }
    }

    private void AddLineAndCheckForFocus(string line)
    {
        ConsoleHandler.init.AddLine(string.Format(line));
        if (autoFocus)
        {
            ConsoleHandler.init.Focus();
        }
        nCount++;
    }

    private void OnParsedConsoleEvent(string command, string[] arguments)
    {
        if (command == "destroy")
        {
            Dictionary<string, string> subCommands = ConsoleParser.ArgumentsToSubCommands(arguments);

            type = "Unit";
            nCount = 0;
            nMax = 1;
            targetLayer = 1;
            autoFocus = true;

            bool notCancel = true;

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
                    case "cancel":
                        nMax = 0;
                        notCancel = false;
                        ConsoleHandler.init.AddLine("destroy command cancelled");
                        break;
                }
            }

            if(notCancel)
            {
                ConsoleHandler.init.AddLine("Click a tile/unit to destroy...");
            }
        }
    }
}
