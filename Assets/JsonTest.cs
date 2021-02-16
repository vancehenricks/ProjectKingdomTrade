/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2021
 */

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TestInfo
{
    public string tileType;
    public string subType;
    public long tileId;
    public string tileName;
    public float localTemp;
    public Vector2 test2;
    public float travelTime;
    public int minChance;
    public int maxChance;
    public int units;
    public bool selected;
    public Vector2 tileLocation;
}

public class JsonTest : MonoBehaviour
{
    private void Start()
    {
        TestInfo testInfo = new TestInfo();

        string json = JsonUtility.ToJson(testInfo);

        StreamWriter writer = File.CreateText(Path.Combine(Application.streamingAssetsPath, "Config/sample.json"));

        foreach (string line in Tools.JsonBeautify(json))
        {
            writer.WriteLine(line);
        }

        writer.Close();
    }
}
