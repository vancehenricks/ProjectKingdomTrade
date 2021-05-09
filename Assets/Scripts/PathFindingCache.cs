/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, September 2020
 */

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
        string keyword = startPoint.transform.position + "," + endPoint.transform.position;

        foreach (string key in cache.Keys)
        {
            if (key.Contains(keyword))
            {
                return cache[key];
            }
        }

        return null;
    }

    public bool ContainsKey(TileInfo startPoint , TileInfo endPoint)
    {
        return cache.ContainsKey(startPoint.transform.position + "," + endPoint.transform.position) ||
            cache.ContainsKey(endPoint.transform.position + "," + startPoint.transform.position);
    }

    public void Add(TileInfo startPoint, TileInfo endPoint, List<TileInfo> generatedPoints)
    {
        if (cache.ContainsKey(startPoint.transform.position + "," + endPoint.transform.position))
        {
            cache.Remove(startPoint.transform.position + "," + endPoint.transform.position);
            cache.Remove(endPoint.transform.position + "," + startPoint.transform.position);
        }

        cache.Add(startPoint.transform.position + "," + endPoint.transform.position, generatedPoints);
        List<TileInfo> reverse = new List<TileInfo>(generatedPoints);
        reverse.Reverse();
        reverse.Add(startPoint);

        if (cache.Count > maxCache)
        {
            cache.Remove(cache.Keys.First());
        }

        cache.Add(endPoint.transform.position + "," + startPoint.transform.position, reverse);
    }
}
