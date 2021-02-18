/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileList : MonoBehaviour
{
    public static Dictionary<long, TileInfo> tileInfos;
    public static Dictionary<long, TileInfo> generatedUnits;
    public static Dictionary<Vector2, TileInfo> generatedTowns;
    public static Dictionary<Vector2, TileInfo> generatedTiles;

    private void Awake()
    {
        tileInfos = new Dictionary<long, TileInfo>();
        generatedTowns = new Dictionary<Vector2, TileInfo>();
        generatedTiles = new Dictionary<Vector2, TileInfo>();
        generatedUnits = new Dictionary<long, TileInfo>();
    }

    private void OnDestroy()
    {
        tileInfos.Clear();
        generatedTowns.Clear();
        generatedTiles.Clear();
        generatedUnits.Clear();
    }

    public static void Add(TileInfo tileInfo)
    {

        if (tileInfo.tileType == "Town")
        {
            ReplaceTown(tileInfo);
            ReplaceTile(tileInfo);
        }
        else if (tileInfo.tileType != "Unit")
        {
            ReplaceTile(tileInfo);
        }
        else if (tileInfo.tileType == "Unit")
        {
            generatedUnits.Add(tileInfo.tileId, tileInfo);
        }

        tileInfos.Add(tileInfo.tileId, tileInfo);
    }

    public static void Remove(TileInfo tileInfo)
    {
        if (tileInfo.tileType == "Town")
        {
            generatedTiles.Remove(tileInfo.tileLocation);
            generatedTowns.Remove(tileInfo.tileLocation);
        }
        else if (tileInfo.tileType != "Unit")
        {
            generatedTiles.Remove(tileInfo.tileLocation);
        }
        else if (tileInfo.tileType == "Unit")
        {
            generatedUnits.Remove(tileInfo.tileId);
        }

        tileInfos.Remove(tileInfo.tileId);
    }

    private static void ReplaceTile(TileInfo tileInfo)
    {
        try
        {
            generatedTiles.Remove(tileInfo.tileLocation);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }

        generatedTiles.Add(tileInfo.tileLocation, tileInfo);
    }

    private static void ReplaceTown(TileInfo tileInfo)
    {
        try
        {
            generatedTowns.Remove(tileInfo.tileLocation);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }

        generatedTowns.Add(tileInfo.tileLocation, tileInfo);
    }

    public static List<TileInfo> GetNeighbours(TileInfo tile)
    {
        List<TileInfo> tiles = new List<TileInfo>();

        for (int i = 0; i < (int)Direction.Length; i++)
        {
            TileInfo tileInfo = GetObjectFrom(tile.tileLocation, (Direction)i);
            if (tileInfo == null) continue;

            tiles.Add(tileInfo);
        }

        return tiles;
    }

    public static TileInfo GetObjectFrom(Vector2 tileLocation, Direction direction)
    {
        Vector2 key = Vector2.zero;
        switch (direction)
        {
            case Direction.Up:
                key = new Vector2(tileLocation.x - 1, tileLocation.y);
                break;
            case Direction.UpRight:
                key = new Vector2(tileLocation.x - 1, tileLocation.y + 1);
                break;
            case Direction.Right:
                key = new Vector2(tileLocation.x, tileLocation.y + 1);
                break;
            case Direction.DownRight:
                key = new Vector2(tileLocation.x + 1, tileLocation.y + 1);
                break;
            case Direction.Down:
                key = new Vector2(tileLocation.x + 1, tileLocation.y);
                break;
            case Direction.DownLeft:
                key = new Vector2(tileLocation.x + 1, tileLocation.y - 1);
                break;
            case Direction.Left:
                key = new Vector2(tileLocation.x, tileLocation.y - 1);
                break;
            case Direction.UpLeft:
                key = new Vector2(tileLocation.x - 1, tileLocation.y - 1);
                break;
        }

        TileInfo tileInfo = null;
        generatedTiles.TryGetValue(key, out tileInfo);

        return tileInfo;
    }
}
