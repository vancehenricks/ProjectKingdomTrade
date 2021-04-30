/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2021
 */

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonTemplate : MonoBehaviour
{
    public OptionGenerator optionGenerator;

    private void Start()
    {
        List<string> options = new List<string>();

        foreach (GameObject option in optionGenerator.options)
        {
            options.Add(option.name);
        }

        TileConfig tileConfig = new TileConfig();
        tileConfig.options = options.ToArray();
        tileConfig.spawnableTile = new string[] { "Land" };
        tileConfig.nonWalkable = new Walkable[] { new Walkable("Sea", 0f, 0f) };
        tileConfig.spawnDistance = new SpawnDistance[] { new SpawnDistance() };
        tileConfig.upgrades = new Upgrade[] { new Upgrade() };
        
        Tools.WriteTileConfig(tileConfig, "template.json");
    }

}
