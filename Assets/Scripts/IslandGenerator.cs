/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, April 2021
 */


using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IslandGenerator : MonoBehaviour
{
    private static IslandGenerator _init;

    public static IslandGenerator init
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
        MapGenerator.init.Add(Generate, 0f);
    }
    private void Generate()
    {
        CDebug.Log(this, "Generating Island");



        Vector3 placeHolder = MapGenerator.init.placeHolderTile.transform.position;
        Vector3 originPos = placeHolder;

        float[,] noiseMap = GenerateNoise();

        for (int y = 0; y < MapGenerator.init.height; y++)
        {
            for (int x = 0; x < MapGenerator.init.width; x++)
            {
                GameObject subGrid = Instantiate<GameObject>(MapGenerator.init.placeHolderTile, MapGenerator.init.grid);
                subGrid.SetActive(true);

                List<TileInfo> baseTiles = TileConfigHandler.init.baseTiles.Values.ToList<TileInfo>();
                TileInfo baseTile = GetBaseTile(noiseMap[x, y], baseTiles);
                TileInfo newTile = Instantiate(baseTile, subGrid.transform);
                newTile.name = baseTile.name;
                newTile.transform.position = placeHolder;
                newTile.tileLocation = new Vector2Int(x, y);
                subGrid.name = x + "_" + y;
                newTile.Initialize();
                newTile.gameObject.SetActive(true);

                placeHolder = new Vector3(placeHolder.x + Tools.tileSize, placeHolder.y, placeHolder.z);
            }

            placeHolder = new Vector3(originPos.x, placeHolder.y - Tools.tileSize, originPos.z);
        }
    }

    public float[,] GenerateNoise()
    {
        float[,] noiseMap = new float[MapGenerator.init.width, MapGenerator.init.height];
        for (int y = 0; y < MapGenerator.init.height; y++)
        {
            for (int x = 0; x < MapGenerator.init.width; x++)
            {
                float xCoord = MapGenerator.init.xOffset + ((x / (float)MapGenerator.init.width) * MapGenerator.init.scale);
                float yCoord = MapGenerator.init.yOffset + ((y / (float)MapGenerator.init.height) * MapGenerator.init.scale);
                noiseMap[x, y] = Mathf.PerlinNoise(xCoord, yCoord) * 2 - 1;
            }
        }

        return noiseMap;
    }

    public TileInfo GetBaseTile(float spawnHeight, List<TileInfo> baseTiles)
    {
        //sortedlist is ascending order
        SortedList<float, TileInfo> candidateTiles = new SortedList<float, TileInfo>();

        candidateTiles.Add(baseTiles[0].spawnChance, baseTiles[0]);

        foreach (TileInfo baseTile in baseTiles)
        {
            if (baseTile.spawnLayer != 0) continue;

            if (spawnHeight >= baseTile.spawnHeightMin && spawnHeight <= baseTile.spawnHeightMax)
            {
                candidateTiles.Add(MapGenerator.init.GetUniqueSpawnChance(candidateTiles, baseTile), baseTile);
            }
        }

        foreach (TileInfo candidateTile in candidateTiles.Values)
        {
            if (Random.Range(0f, 1f) <= candidateTile.spawnChance)
            {
                return candidateTile;
            }
        }

        return TileConfigHandler.init.baseTiles["Sea"];
    }
}