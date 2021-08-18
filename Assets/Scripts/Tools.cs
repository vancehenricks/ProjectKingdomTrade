/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, August 2020
 */


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

    public static List<T> WhiteList<T>(List<T> _tileInfos, List<T> include) where T : TileInfo
    {
        List<T> tileInfos = new List<T>();

        foreach (T tile in _tileInfos)
        {
            foreach (T inc in include)
            {
                CDebug.Log(nameof(Tools), inc.name, LogType.Warning);

                if (inc.tileType == tile.tileType)
                {
                    tileInfos.Add(tile);
                    break;
                }
            }
        }

        return tileInfos;
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

    public static int TileLocationDistance(TileInfo tile1, TileInfo tile2)
    {
        int distance = 0;

        //try
        //{
            distance = Mathf.RoundToInt(Vector2Int.Distance(tile1.tileLocation, tile2.tileLocation));
        //}
        //catch (System.Exception e)
        //{
        //    CDebug.Log(nameof(Tools), e,LogType.Warning);
        //    distance = Mathf.RoundToInt(Vector2.Distance(tile1.transform.position, tile2.transform.position) / Tools.tileSize);
        //}

        return distance;
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
        newTile.name = baseTile.name;
        newTile.transform.position = target.transform.position;
        newTile.tileLocation = target.tileLocation;

        CDebug.Log(nameof(Tools), "newTile.tileLocation=" + newTile.tileLocation + " " + "target.tileLocation=" + target.tileLocation, LogType.Warning);

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

    public static void SetDirection(Transform currentTransform, Vector3 newPosition)
    {
        Quaternion rotation = currentTransform.rotation;
        Vector3 currentPosition = currentTransform.position;

        //left
        if (newPosition.x == 0) return;

        if (currentPosition.x > newPosition.x)
        {
            currentTransform.rotation = new Quaternion(rotation.x, 180, rotation.z, rotation.w);
        }

        //right
        if (currentPosition.x < newPosition.x)
        {
            currentTransform.rotation = new Quaternion(rotation.x, 0, rotation.z, rotation.w);
        }
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

    public static int GetNumberOfTiles(float width, float height)
    {
        return (int)(width * height);
    }

    public static Vector2Int ParseLocation(string coordinates)
    {
        string[] locArray = coordinates.Split(',');
        int x, y;

        int.TryParse(locArray[0], out x);
        int.TryParse(locArray[1], out y);

        return new Vector2Int(x, y);
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

    public static bool IsWithinCameraView(OcclusionValue occlusion)
    {
        if (occlusion.screenPos.x >= -occlusion.overflow &&
            occlusion.screenPos.x <= occlusion.screenSize.x + occlusion.overflow &&
            occlusion.screenPos.y >= -occlusion.overflow &&
            occlusion.screenPos.y <= occlusion.screenSize.y + occlusion.overflow)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static List<BaseInfo> ConvertTileToBaseInfo (List<TileInfo> tInfos)
    {
        List<BaseInfo> bInfos = new List<BaseInfo>();

        foreach(BaseInfo bInfo in tInfos)
        {
            bInfos.Add(bInfo);
        }

        return bInfos;
    }

    public static List<TileInfo> ConvertBaseToTileInfo (List<BaseInfo> bInfos)
    {
        List<TileInfo> tInfos = new List<TileInfo>();

        foreach(TileInfo tInfo in bInfos)
        {
            tInfos.Add(tInfo);
        }

        return tInfos;
    }
}
