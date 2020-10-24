﻿/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, August 2020
 */

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

    public static List<T> Convert<B,T>(List<B> tileInfos)
    {
        List<T> infos = new List<T>();

        foreach (B tileInfo in tileInfos)
        {
            try
            {
                Debug.Log("Attempting to Downcast");
                T info = (T)System.Convert.ChangeType(tileInfo, typeof(B));
                infos.Add(info);
            }
            catch
            {
                Debug.Log("Conversion error try something else");
                T info = (T)System.Convert.ChangeType(tileInfo, typeof(T));
                infos.Add(info);
            }
        }

        //Debug.Log("infos.Count="+infos.Count);

        return infos;
    }

    public static List<TileInfo> GetNeighbours(TileInfo tile)
    {
        List<TileInfo> tiles = new List<TileInfo>();

        for (int i = 0; i < (int)Direction.Length; i++)
        {
            GameObject obj = MapGenerator.init.GetObjectFrom(tile.tileLocation, (Direction)i);
            if (obj == null) continue;

            tiles.Add(obj.GetComponent<TileInfo>());
        }

        return tiles;
    }

    public static List<TileInfo> WhiteList(List<TileInfo> _tileInfos, List<TileInfo> include)
    {
        List<TileInfo> tileInfos = new List<TileInfo>();

        foreach (TileInfo tile in _tileInfos)
        {
            foreach (TileInfo inc in include)
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

    public static List<TileInfo> RemoveDuplicates(List<TileInfo> selectedTiles, List<TileInfo> targetList)
    {
        List<TileInfo> sanitizeList = new List<TileInfo>();

        foreach (TileInfo tile in selectedTiles)
        {
            if (!targetList.Contains(tile))
            {
                sanitizeList.Add(tile);
            }
        }

        return sanitizeList;
    }

    public static List<TileInfo> RemoveNulls(List<TileInfo> tiles)
    {
        List<TileInfo> sanitizedTiles = new List<TileInfo>();

        foreach (TileInfo tile in tiles)
        {
            if (tile == null) continue;
            sanitizedTiles.Add(tile);
        }

        return sanitizedTiles;
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
}
