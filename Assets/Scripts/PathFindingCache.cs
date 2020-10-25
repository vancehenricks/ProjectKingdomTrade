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
    public static Dictionary<string, List<TileInfo>> cache;

    private void Start()
    {
        cache = new Dictionary<string, List<TileInfo>>();
    }

    public static List<TileInfo> RetrieveTileInfos(TileInfo endPoint)
    {
        string keyword = "," + endPoint.transform.position;

        foreach (string key in cache.Keys)
        {
            if (key.Contains(keyword))
            {
                return cache[key];
            }
        }

        return null;
    }

    public static TileInfo FindIntersectingTile(List<TileInfo> tileInfos, TileInfo originTile)
    {
        return FindNearest(tileInfos, originTile, 3, true)[0];
    }

    public static List<TileInfo> FindNearest(List<TileInfo> tileInfos, TileInfo originTile, int maxDistance, bool returnTileInfoOnly = false)
    {
        if (tileInfos == null) return null;

        List<TileInfo> final = new List<TileInfo>();

        int nearestIndex = 0;
        float lowestDistance = maxDistance;

        for (int i = 0; i < tileInfos.Count; i++)
        {
            float distance = Vector2.Distance(originTile.tileLocation, tileInfos[i].tileLocation);

            Debug.Log("FindNearest distance=" + distance);

            if (distance <= maxDistance && distance <= lowestDistance)
            {
                lowestDistance = distance;
                nearestIndex = i;
            }
        }

        for (int i = nearestIndex; i < tileInfos.Count; i++)
        {
            Debug.Log("Found some");
            final.Add(tileInfos[i]);

            if (returnTileInfoOnly) return final;
        }

        if (final.Count == 0)
        {
            final = null;
        }

        return final;
    }
}
