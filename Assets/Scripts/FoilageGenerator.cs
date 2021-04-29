/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, April 2021
 */

using DebugHandler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FoilageGenerator : MonoBehaviour
{
    public delegate List<T> LiveList<T>();

    private static FoilageGenerator _init;

    public static FoilageGenerator init
    {
        get { return _init; }
        private set { _init = value; }
    }

    private void Awake()
    {
        init = this;
    }

    private void Start()
    {
        MapGenerator.init.Add(Generate, 7f);
    }
    private void Generate()
    {
        CDebug.Log(this,"Generating Foilage");

        List<TileInfo> generatedTiles = TileList.init.generatedTiles.Values.ToList<TileInfo>();

        Generate(TileConfigHandler.init.baseTiles.Values.ToList<TileInfo>(), generatedTiles, () => {
            return TileList.init.generatedTiles.Values.ToList<TileInfo>(); 
        });
    }

    public void Generate(List<TileInfo> foilageTiles, List<TileInfo> replaceTiles, LiveList<TileInfo> checkDistance, bool newPlayer = false)
    {
        Dictionary<string, SortedList<float, TileInfo>> foilageGroup = GetFoilageGroup(foilageTiles);

        foreach (KeyValuePair<string, SortedList<float, TileInfo>> baseTiles in foilageGroup)
        {
            foreach (TileInfo tileInfo in replaceTiles)
            {
                if (tileInfo.subType != baseTiles.Key && tileInfo.tileType != baseTiles.Key) continue;

                TileInfo candidate = GetBaseTile(baseTiles.Value);

                if (candidate == null) continue;

                if (!CheckForDistance(candidate, tileInfo, checkDistance())) continue;

                Tools.ReplaceTile(candidate, tileInfo, newPlayer);
            }
        }
    }

    public TileInfo GetBaseTile(SortedList<float, TileInfo> baseTiles)
    {
        foreach (KeyValuePair<float, TileInfo> baseTile in baseTiles)
        {
            if (Random.Range(0f, 1f) <= baseTile.Key)
            {
               return baseTile.Value;
            }
        }

        return null;
    }

    public Dictionary<string, SortedList<float, TileInfo>> GetFoilageGroup(List<TileInfo> _baseTiles)
    {
        Dictionary<string, SortedList<float, TileInfo>> foilageGroup = new Dictionary<string, SortedList<float, TileInfo>>();

        foreach (TileInfo baseTile in _baseTiles)
        {
            if (baseTile.spawnableTile.Count == 0) continue;

            foreach (string spawnable in baseTile.spawnableTile)
            {
                if (foilageGroup.ContainsKey(spawnable))
                {
                    foilageGroup[spawnable].Add(MapGenerator.init.GetUniqueSpawnChance(foilageGroup[spawnable], baseTile), baseTile);
                }
                else
                {
                    SortedList<float, TileInfo> baseTiles = new SortedList<float, TileInfo>();
                    baseTiles.Add(baseTile.spawnChance, baseTile);
                    foilageGroup.Add(spawnable, baseTiles);
                }
            }
        }

        return foilageGroup;
    }

    public bool CheckForDistance(TileInfo candidate, TileInfo tileInfo, List<TileInfo> generatedTiles)
    {

        foreach (TileInfo tileInfo2 in generatedTiles)
        {
            int spawnX = (int)(MapGenerator.init.width * candidate.spawnDistance);
            int spawnY = (int)(MapGenerator.init.height * candidate.spawnDistance);

            int distance = Tools.TileLocationDistance(tileInfo, tileInfo2);

            //CDebug.Log(this, "distance=" + distance + " spawnX=" + spawnX + " spawnY=" + spawnY + " candidate=" + candidate.tileId + " tileInfo=" + tileInfo.tileId + " tileInfo2=" + tileInfo2.tileId);

            if (distance < spawnX || distance < spawnY)
            {
                return false;
            }
        }

        return true;
    }
}
