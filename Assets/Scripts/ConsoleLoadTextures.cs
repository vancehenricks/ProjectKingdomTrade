/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleLoadTextures : ConsoleCommand
{
    public override void Initialize()
    {
        Dictionary<string, string> subCommands = new Dictionary<string, string>();
        subCommands.Add("cancel", "");
        subCommands.Add("help", "");
        ConsoleHandler.init.AddCommand("load-textures", subCommands);
        ConsoleHandler.init.AddCache("load-textures");
    }

    private IEnumerator ExecuteCommand()
    {
        LoadingHandler.init.SetActive(true);
        LoadingHandler.init.Set(1f, "Loading Textures...");
        TextureHandler.init.LoadTextures();
        yield return new WaitForSeconds(1f);
        ConsoleHandler.init.AddCache(ConsoleHandler.init.previousCommand);
        yield return new WaitForSeconds(1f);
        ConsoleHandler.init.AddLine("Textures Loaded!");
        LoadingHandler.init.SetActive(false);
    }

    public override void OnParsedConsoleEvent(string command, string[] arguments)
    {
        if (command == "load-textures")
        {
            Dictionary<string, string> subCommands = ConsoleParser.ArgumentsToSubCommands(arguments);

            foreach (string subCommand in subCommands.Keys)
            {
                switch (subCommand)
                {
                    case "cancel":
                        return;
                    case "help":
                    default:
                        ConsoleHandler.init.DisplaySubCommands("load-textures");
                        return;
                }
            }

            StopAllCoroutines();
            StartCoroutine(ExecuteCommand());
        }
    }
}
