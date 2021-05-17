/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, September 2020
 */

using DebugHandler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFindingCache : MonoBehaviour
{
    private static PathFindingCache _init;

    public static PathFindingCache init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public int maxCache;
    private Dictionary<string, List<TileInfo>> cache;

    private void Awake()
    {
        init = this;
    }

    private void Start()
    {
        cache = new Dictionary<string, List<TileInfo>>();
    }

    private void OnDestroy()
    {
        cache.Clear();
    }

    public List<TileInfo> RetrieveTileInfos(TileInfo startPoint, TileInfo endPoint)
    {
        Vector3Int _start = Vector3Int.FloorToInt(startPoint.transform.position);
        Vector3Int _end = Vector3Int.FloorToInt(endPoint.transform.position);

        string keyword = _start + "," + _end;

        if (cache.ContainsKey(keyword))
        {
            return cache[keyword];
        }

        return null;
    }

    public bool ContainsKey(TileInfo startPoint , TileInfo endPoint)
    {
        Vector3Int _start = Vector3Int.FloorToInt(startPoint.transform.position);
        Vector3Int _end = Vector3Int.FloorToInt(endPoint.transform.position);

        return cache.ContainsKey(_start + "," + _end) ||
            cache.ContainsKey(_start + "," + _end);
    }

    public void Add(TileInfo startPoint, TileInfo endPoint, List<TileInfo> generatedPoints)
    {
        Vector3Int _start = Vector3Int.FloorToInt(startPoint.transform.position);
        Vector3Int _end = Vector3Int.FloorToInt(endPoint.transform.position);

        if (cache.ContainsKey(_start + "," + _end))
        {
            cache.Remove(_start + "," + _end);
            cache.Remove(_end + "," + _start);
        }

        try
        {
            cache.Add(_start + "," + _end, generatedPoints);
            List<TileInfo> reverse = new List<TileInfo>(generatedPoints);
            reverse.Reverse();
            reverse.Add(startPoint);

            if (cache.Count > maxCache)
            {
                cache.Remove(cache.Keys.First());
            }

            cache.Add(_end + "," + _start, reverse);
        }
        catch (System.Exception e)
        {
            CDebug.Log(this,e,LogType.Warning);
        }
    }
}
