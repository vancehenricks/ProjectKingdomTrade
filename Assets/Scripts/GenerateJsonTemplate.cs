/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2021
 */

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GenerateJsonTemplate : MonoBehaviour
{
    private void Start()
    {
        TileInfoJson tileInfoJson = new TileInfoJson();

        string json = JsonUtility.ToJson(tileInfoJson);

        StreamWriter writer = File.CreateText(Path.Combine(Application.streamingAssetsPath, "Config/tile-sample.json"));

        foreach (string line in Tools.JsonBeautify(json))
        {
            writer.WriteLine(line);
        }

        writer.Close();
    }
}
