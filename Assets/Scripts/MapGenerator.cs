/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, January 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class MapGenerator : Pipeline
{
    private static MapGenerator _init;

    public static MapGenerator init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public GameObject placeHolderTile;
    public RectTransform grid;
    public bool useGridSize;
    public int width;
    public int height;
    public float xOffset;
    public float yOffset;
    public float scale;

    protected override void Awake()
    {
        init = this;
        base.Awake();
    }

    public void Initialize(float _xOffset, float _yOffset, float _scale)
    {
        xOffset = _xOffset;
        yOffset = _yOffset;
        scale = _scale;

        foreach (TileInfo tile in TileList.init.generatedTiles.Values)
        {
            Destroy(tile);
        }
        TileList.init.generatedTiles.Clear();

        if (useGridSize)
        {
            RectTransform rectPlaceHolder = placeHolderTile.GetComponent<RectTransform>();
            width = Mathf.RoundToInt(grid.rect.width / rectPlaceHolder.rect.width);
            height = Mathf.RoundToInt(grid.rect.height / rectPlaceHolder.rect.height);
        }

        base.Execute();
    }

    public float GetUniqueSpawnChance(SortedList<float, TileInfo> candidateTiles, TileInfo baseTile)
    {
        float spawnChance = baseTile.spawnChance;

        while (candidateTiles.ContainsKey(spawnChance))
        {
            if (Random.Range(0f, 1f) > 0.5f)
            {
                candidateTiles[spawnChance] = baseTile;
            }

            spawnChance = baseTile.spawnChance + Random.Range(0f, 1f);
        }

        return spawnChance;
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

    public Dictionary<string, SortedList<float, TileInfo>> GetFoilageGroup()
    {
        Dictionary<string, SortedList<float, TileInfo>> foilageGroup = new Dictionary<string, SortedList<float, TileInfo>>();

        foreach (TileInfo baseTile in TileConfigHandler.init.baseTiles.Values)
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
}
