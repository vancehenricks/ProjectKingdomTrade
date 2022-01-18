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
    protected override string SetCommand()
    {
        return "load-textures";
    }

    public override void Initialize(Dictionary<string, string> subCommands)
    {
        //subCommands.Add("cancel", "");
        subCommands.Add("help", "");
    }

    private IEnumerator ExecuteCommand()
    {
        LoadingHandler.init.SetActive(true);
        LoadingHandler.init.Set(1f, "Loading Textures...");
        TextureHandler.init.Load();
        yield return new WaitForSeconds(1f);
        ConsoleHandler.init.AddCache(ConsoleHandler.init.previousCommand);
        yield return new WaitForSeconds(1f);
        ConsoleHandler.init.AddLine("Textures Loaded!");
        LoadingHandler.init.SetActive(false);
    }

    public override void OnParsedConsoleEvent( Dictionary<string, string> subCommands, string[] arguments, params object[] objects)
    {
        foreach (string subCommand in subCommands.Keys)
        {
            switch (subCommand)
            {
                //case "cancel":
                //    return;
                case "help":
                default:
                    ConsoleHandler.init.DisplaySubCommands("load-textures");
                    return;
            }
        }

        StartCoroutine(ExecuteCommand());
    }
}
