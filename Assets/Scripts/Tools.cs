/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, August 2020
 */

using DebugHandler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class Tools : MonoBehaviour
{
    private static float _tileSize = 25f;

    public static float tileSize
    {
        get
        {
            return _tileSize;
        }
        private set
        {
            _tileSize = value;
        }
    }

    //on save make sure to save this
    private static long _UniqueId;
    public static long UniqueId
    {
        get
        {
            return _UniqueId++;
        }

        private set
        {
            _UniqueId = value;
        }
    }

    private void OnDestroy()
    {
        _UniqueId = 0;
    }

    /*
		up		    current index - width	
		down	    current index + width	
		left	    current index - 1	
		right	    current index + 1
        up-left     current index - width)-1
        up-right    current index - width)+1
        down-left   current index + width)-1
        down-right  current index - width)+1
	*/

    public static List<T> Convert<B, T>(List<B> tileInfos)
    {
        List<T> infos = new List<T>();

        foreach (B tileInfo in tileInfos)
        {
            infos.Add(Convert<B, T>(tileInfo));
        }

        //Debug.Log("infos.Count="+infos.Count);

        return infos;
    }

    public static T Convert<B, T>(B tileInfo)
    {
        try
        {
            CDebug.Log(nameof(Tools), "Attempting to Downcast");
            T info = (T)System.Convert.ChangeType(tileInfo, typeof(B));
            return info;
        }
        catch (System.Exception e)
        {
            CDebug.Log(nameof(Tools), e, LogType.Warning);
            T info = (T)System.Convert.ChangeType(tileInfo, typeof(T));
            return info;
        }
    }

    public static List<T> WhiteList<T>(List<T> _tileInfos, List<T> include) where T : TileInfo
    {
        List<T> tileInfos = new List<T>();

        foreach (T tile in _tileInfos)
        {
            foreach (T inc in include)
            {
                if (inc.tileType == tile.tileType)
                {
                    tileInfos.Add(tile);
                    break;
                }
            }
        }

        return tileInfos;
    }

    public static int Exist<T>(List<T> list, T key) where T : TileInfo
    {

        int index = -1;

        foreach (TileInfo tile in list)
        {
            index++;

            if (tile.tileId == key.tileId)
            {
                return index;
            }
        }

        return -1;
    }


    public static List<T> MergeList<T>(List<T> selectedTiles, List<T> targetList) where T : TileInfo
    {
        List<T> sanitizeList = new List<T>();

        foreach (T tile in selectedTiles)
        {
            if (!targetList.Contains(tile))
            {
                sanitizeList.Add(tile);
            }
        }

        return sanitizeList;
    }

    public static void RemoveNulls(List<TileInfo> tiles)
    {
        foreach (TileInfo tile in tiles)
        {
            if (tile == null)
            {
                tiles.Remove(tile);
            }
        }
    }

    public static int TileLocationDistance(TileInfo tile1, TileInfo tile2)
    {
        int distance = 0;

        try
        {
            distance = (int)Math.Round(Vector2.Distance(tile1.tileLocation, tile2.tileLocation), MidpointRounding.AwayFromZero);
        }
        catch (System.Exception e)
        {
            CDebug.Log(nameof(Tools), e,LogType.Warning);
            distance = (int)Math.Round(Vector2.Distance(tile1.transform.position, tile2.transform.position) / Tools.tileSize, MidpointRounding.AwayFromZero);
        }

        return distance;
    }

    public static bool HasNulls(List<TileInfo> tiles)
    {
        foreach (TileInfo tile in tiles)
        {
            if (tile == null) return true;
        }

        return false;
    }

    public static void Merge(UnitInfo unit1, UnitInfo unit2)
    {
        unit2.unit += unit1.unit;
        //need to deal how to calculate other values e.g. attackDistance, killChance and deathChance
        Destroy(unit1.gameObject);
    }

    public static void Split(UnitInfo baseTile)
    {
        GameObject clone = Instantiate(baseTile.gameObject, baseTile.transform.parent);
        UnitInfo unitInfo = clone.GetComponent<UnitInfo>();
        int result1 = baseTile.unit / 2;
        int result2 = baseTile.unit - result1;
        // same with merge need to calculate other stats also;
        unitInfo.unit = result1;
        baseTile.unit = result2;
        unitInfo.Initialize();
    }

    public static TileInfo ReplaceTile(TileInfo baseTile, TileInfo target, bool isPlayer = false)
    {
        TileInfo newTile = Instantiate(baseTile, target.transform.parent);
        newTile.transform.position = target.transform.position;
        newTile.tileLocation = target.tileLocation;

        //transfer all stats from target to newTile probably a function in TileInfo or do it here
        target.Destroy();

        if (isPlayer)
        {
            newTile.playerInfo = PlayerList.init.Instantiate();
        }

        newTile.Initialize();
        newTile.gameObject.SetActive(true);
        return newTile;
    }

    public static string ConvertToSymbols(int value)
    {
        string _value = value.ToString("N");
        string[] split = _value.Split(',');
        if (value > 100000000)
        {
            return "????";
        }
        else if (value >= 1000000)
        {
            return split[0] + "M";
        }
        else if (value >= 1000)
        {
            return split[0] + "K";
        }

        return value + "";
    }

    public static int GetNumberOfTiles(float w1, float h1, float w2, float h2)
    {
        float width = w1 / w2;
        float height = h1 / h2;

        return (int)(width * height);
    }

    public static Vector2 ParseLocation(string coordinates)
    {
        string[] locArray = coordinates.Split(',');
        float x, y;

        float.TryParse(locArray[0], out x);
        float.TryParse(locArray[1], out y);

        return new Vector2(x, y);
    }

    public static void WriteConfig(object tileConfig, string fileName)
    {
        string json = JsonUtility.ToJson(tileConfig);

        StreamWriter writer = File.CreateText(Path.Combine(Application.streamingAssetsPath, fileName));

        foreach (string line in Tools.JsonBeautify(json))
        {
            writer.WriteLine(line);
        }

        writer.Close();
    }

    public static void WriteTileConfig(TileConfig tileConfig, string fileName)
    {
        WriteConfig(tileConfig, Path.Combine("Config", "Tiles", fileName));
    }

    private static Regex regexSplit;

    public static List<string> JsonBeautify(string json)
    {
        string[] jsonArray = Regex.Split(json, @"(\{|\}|\[|\]|,)", RegexOptions.Compiled);
        List<string> final = new List<string>();
        string space = "";
        for (int i = 0; i < jsonArray.Length; i++)
        {
            if (jsonArray[i].Contains("{") || jsonArray[i].Contains("["))
            {
                final.Add(space + jsonArray[i]);
                space += " ";
            }
            else if (jsonArray[i].Contains("}") || jsonArray[i].Contains("]"))
            {
                if (space.Length > 0)
                {
                    space = space.Remove(space.Length - 1);
                }
                else
                {
                    space = "";
                }

                final.Add(space + jsonArray[i]);
            }
            else if (jsonArray[i].Length > 0 && !jsonArray[i].Contains(","))
            {
                final.Add(space + jsonArray[i]);
            }
            else if (jsonArray[i].Contains(","))
            {
                final[final.Count - 1] += ",";
            }
        }

        return final;
    }
}
