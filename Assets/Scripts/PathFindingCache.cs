/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, September 2020
 */


using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
    public ConcurrentDictionary<string, List<TileInfo>> cache;

    private void Awake()
    {
        init = this;
    }

    private void Start()
    {
        cache = new ConcurrentDictionary<string, List<TileInfo>>();
    }

    private void OnDestroy()
    {
        cache.Clear();
    }

    public void Add(TileInfo startPoint, List<TileInfo> generatedPoints)
    {
        Vector2Int _start = startPoint.tileLocation;
        Vector2Int _end = generatedPoints[generatedPoints.Count-1].tileLocation;

        List<TileInfo> temp = new List<TileInfo>(generatedPoints);

        List<TileInfo> delResult;

        if(!cache.TryRemove(_start + "," + _end, out delResult))
        {
            if(cache.Count >= maxCache)
            {
                if(!cache.TryRemove(cache.Keys.First<string>(), out delResult) && delResult != null)
                {
                    CDebug.Log(this, "Remove failed for cache.Keys.First=" + cache.Keys.First<string>(), LogType.Error);
                }
            }
        }

        cache.TryAdd(_start + "," + _end, temp);
    }
}
