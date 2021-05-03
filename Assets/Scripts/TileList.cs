/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2021
 */

using DebugHandler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileList : MonoBehaviour
{
    private static TileList _init;
    public static TileList init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public Dictionary<Vector3Int, Transform> subGrids;
    public Dictionary<long, TileInfo> tileInfos;
    public Dictionary<long, TileInfo> generatedUnits;
    public Dictionary<Vector2Int, TileInfo> generatedTowns;
    public Dictionary<Vector2Int, TileInfo> generatedTiles;

    private void Awake()
    {
        init = this;
        subGrids = new Dictionary<Vector3Int, Transform>();
        tileInfos = new Dictionary<long, TileInfo>();
        generatedTowns = new Dictionary<Vector2Int, TileInfo>();
        generatedTiles = new Dictionary<Vector2Int, TileInfo>();
        generatedUnits = new Dictionary<long, TileInfo>();
    }

    public void Add(TileInfo tileInfo)
    {

        if (tileInfo.tileType == "Town")
        {
            ReplaceTile(tileInfo, generatedTowns);
            ReplaceTile(tileInfo, generatedTiles);
        }
        else if (tileInfo.tileType != "Unit")
        {
            ReplaceTile(tileInfo, generatedTiles);
        }
        else if (tileInfo.tileType == "Unit")
        {
            generatedUnits.Add(tileInfo.tileId, tileInfo);
        }

        Vector3Int pos = Vector3Int.FloorToInt(tileInfo.transform.position);
        if (!subGrids.ContainsKey(pos))
        {
            subGrids.Add(pos, tileInfo.transform.parent);
        }

        tileInfos.Add(tileInfo.tileId, tileInfo);
    }


    public void Remove(TileInfo tileInfo)
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

    private void ReplaceTile(TileInfo tileInfo, Dictionary<Vector2Int, TileInfo> generatedList)
    {

        if (generatedList.ContainsKey(tileInfo.tileLocation))
        {
            generatedList[tileInfo.tileLocation] = tileInfo;
        }
        else
        {
            generatedList.Add(tileInfo.tileLocation, tileInfo);
        }
    }

    public List<TileInfo> GetNeighbours(TileInfo tile)
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

    public TileInfo GetObjectFrom(Vector2Int tileLocation, Direction direction)
    {
        Vector2Int key = Vector2Int.zero;
        switch (direction)
        {
            case Direction.Up:
                key = new Vector2Int(tileLocation.x, tileLocation.y-1);
                break;
            case Direction.UpRight:
                key = new Vector2Int(tileLocation.x+1, tileLocation.y-1);
                break;
            case Direction.Right:
                key = new Vector2Int(tileLocation.x+1, tileLocation.y);
                break;
            case Direction.DownRight:
                key = new Vector2Int(tileLocation.x+1, tileLocation.y+1);
                break;
            case Direction.Down:
                key = new Vector2Int(tileLocation.x, tileLocation.y+1);
                break;
            case Direction.DownLeft:
                key = new Vector2Int(tileLocation.x-1, tileLocation.y+1);
                break;
            case Direction.Left:
                key = new Vector2Int(tileLocation.x-1, tileLocation.y);
                break;
            case Direction.UpLeft:
                key = new Vector2Int(tileLocation.x-1, tileLocation.y-1);
                break;
        }

        TileInfo tileInfo = null;
        generatedTiles.TryGetValue(key, out tileInfo);

        return tileInfo;
    }
}
