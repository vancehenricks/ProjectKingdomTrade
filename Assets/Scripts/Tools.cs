/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, August 2020
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools : MonoBehaviour
{

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
            Debug.Log("Attempting to Downcast");
            T info = (T)System.Convert.ChangeType(tileInfo, typeof(B));
            return info;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            Debug.LogError("Conversion error try something else");
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
            Debug.LogError(e);
            distance = (int)Math.Round(Vector2.Distance(tile1.transform.position, tile2.transform.position) / 25, MidpointRounding.AwayFromZero);
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
        unit2.units += unit1.units;
        //need to deal how to calculate other values e.g. attackDistance, killChance and deathChance
        Destroy(unit1.gameObject);
    }

    public static void Split(UnitInfo baseTile)
    {
        GameObject clone = Instantiate(baseTile.gameObject, baseTile.transform.parent);
        UnitInfo unitInfo = clone.GetComponent<UnitInfo>();
        int result1 = baseTile.units / 2;
        int result2 = baseTile.units - result1;
        // same with merge need to calculate other stats also;

        unitInfo.units = result1;
        baseTile.units = result2;
        unitInfo.Initialize();
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

    public static List<string> JsonBeautify(string json)
    {
        string[] jsonArray = json.Split('{');
        List<string> final = new List<string>();
        string space = "";
        for (int i = 0; i < jsonArray.Length; i++)
        {
            if (jsonArray[i].Length > 0)
            {
                final.Add(space + "{");
                space += " ";
            }

            string[] jsonArray2 = jsonArray[i].Split(',');

            for (int ii = 0; ii < jsonArray2.Length; ii++)
            {
                if (jsonArray2[ii].Length > 0 && !jsonArray2[ii].Contains("{") && !jsonArray2[ii].Contains("}")
                    && jsonArray2[ii][jsonArray2[ii].Length - 1] != ':')
                {
                    final.Add(space + jsonArray2[ii] + ",");
                }
                else if (jsonArray2[ii].Length > 0 && jsonArray2[ii][jsonArray2[ii].Length - 1] == ':')
                {
                    final.Add(space + jsonArray2[ii]);
                }

                if (jsonArray2[ii].Contains("}"))
                {
                    string normalized = jsonArray2[ii];

                    while (normalized.Contains("}"))
                    {
                        normalized = normalized.Remove(normalized.Length - 1);
                    }

                    final.Add(space + normalized);

                    normalized = jsonArray2[ii];

                    while (normalized.Contains("}"))
                    {
                        normalized = normalized.Remove(normalized.Length - 1);

                        if (space.Length > 0)
                        {
                            space = space.Remove(space.Length - 1);
                        }
                        else
                        {
                            space = "";
                        }

                        final.Add(space + "}");
                    }
                }
            }
        }

        return final;
    }
}
