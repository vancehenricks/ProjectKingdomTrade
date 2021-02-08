/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, September 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingCache : MonoBehaviour
{
    private static Dictionary<string, List<TileInfo>> cache;

    private void Start()
    {
        cache = new Dictionary<string, List<TileInfo>>();
    }

    private void OnDestroy()
    {
        cache.Clear();
    }

    public static List<TileInfo> RetrieveTileInfos(TileInfo startPoint, TileInfo endPoint)
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

    public static bool ContainsKey(TileInfo startPoint , TileInfo endPoint)
    {
        return cache.ContainsKey(startPoint.transform.position + "," + endPoint.transform.position) ||
            cache.ContainsKey(endPoint.transform.position + "," + startPoint.transform.position);
    }

    public static void Add(TileInfo startPoint, TileInfo endPoint, List<TileInfo> generatedPoints)
    {
        cache.Add(startPoint.transform.position + "," + endPoint.transform.position, generatedPoints);
        List<TileInfo> reverse = new List<TileInfo>(generatedPoints);
        reverse.Reverse();
        reverse.Add(startPoint);
        cache.Add(endPoint.transform.position + "," + startPoint.transform.position, reverse);
    }
}
