﻿/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, April 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnUnit : MonoBehaviour
{
    public GameObject baseUnit;
    public Transform parent;
    public TileInfoRaycaster tileInfoRaycaster;
    //public KeyCode spawnKey;
    public int nCount;
    public int nMax;
    public bool autoFocus;
    public Color color;
    public int attackDistance;
    public bool executeAll;
    public bool notCancel;

    private bool fire1Clicked;
    private Dictionary<string, string> subCommands;

    private void Start()
    {
        ConsoleParser.onParsedConsoleEvent += OnParsedConsoleEvent;
        subCommands = new Dictionary<string, string>();
        subCommands.Add("amount", "1");
        subCommands.Add("color", "#ffffff");
        subCommands.Add("attack-distance", "1");
        subCommands.Add("auto-focus", "true");
        subCommands.Add("execute-all", "");
        subCommands.Add("cancel", "");
        subCommands.Add("help", "");

        ConsoleHandler.init.commands.Add("spawn-unit", subCommands);
        ConsoleHandler.init.AddCache("spawn-unit");
    }

    private void Update()
    {
        if ((Input.GetButtonDown("Fire1") && nCount < nMax) || (fire1Clicked && executeAll && nCount < nMax))
        {
            fire1Clicked = true;
            //make the units random color
            TileInfo tile = tileInfoRaycaster.GetTileInfoFromPos(Input.mousePosition);
            if (tile != null)
            {
                GameObject unit = Instantiate(baseUnit, parent);
                unit.transform.position = tile.transform.position;
                UnitInfo unitInfo = unit.GetComponent<UnitInfo>();
                //unitInfo.tileId = Tools.UniqueId + "";
                unitInfo.color = color == Color.white ? Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f) : color;
                unitInfo.attackDistance = attackDistance;
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
    }

    private void OnParsedConsoleEvent(string command, string[] arguments)
    {
        if (command == "spawn-unit")
        {
            Dictionary<string, string> subCommands = ConsoleParser.ArgumentsToSubCommands(arguments);

            UnitInfo unitInfo = baseUnit.GetComponent<UnitInfo>();

            nMax = 1;
            nCount = 0;
            autoFocus = true;
            color = Color.white;
            attackDistance = unitInfo.attackDistance;
            notCancel = true;
            executeAll = false;
            fire1Clicked = false;

            foreach (string subCommand in subCommands.Keys)
            {
                Debug.Log(subCommand);

                switch (subCommand)
                {
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
            }
        }
    }
}