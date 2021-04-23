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
    private List<TileInfo> baseTiles;

    private void Start()
    {
        MapGenerator.init.Add(Generate, 0f);
    }
    public void Generate()
    {
        Debug.Log("Island Generator");

        baseTiles = TileConfigHandler.init.baseTiles.Values.ToList<TileInfo>();

        Vector3 placeHolder = MapGenerator.init.placeHolderTile.transform.position;
        Vector3 originPos = placeHolder;

        float[,] noiseMap = GenerateNoise();

        for (int y = 0; y < MapGenerator.init.height; y++)
        {
            for (int x = 0; x < MapGenerator.init.width; x++)
            {
                TileInfo newTile = Instantiate(GetBaseTile(noiseMap[x, y]), MapGenerator.init.grid);
                newTile.transform.position = placeHolder;
                newTile.tileLocation = new Vector2(x, y);
                newTile.Initialize();
                newTile.gameObject.SetActive(true);

                placeHolder = new Vector3(placeHolder.x + 25f, placeHolder.y, placeHolder.z);
            }

            placeHolder = new Vector3(originPos.x, placeHolder.y - 25, originPos.z);
        }
    }

    private float[,] GenerateNoise()
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

    private TileInfo GetBaseTile(float spawnHeight)
    {
        //sortedlist is ascending order
        SortedList<float, TileInfo> candidateTiles = new SortedList<float, TileInfo>();

        candidateTiles.Add(baseTiles[0].spawnChance, baseTiles[0]);

        foreach (TileInfo baseTile in baseTiles)
        {
            if (spawnHeight >= baseTile.spawnHeightMin && spawnHeight <= baseTile.spawnHeightMax)
            {
                float spawnChance = baseTile.spawnChance;

            //guarantees no duplication causing exception

            RECHECK:
                if (candidateTiles.ContainsKey(spawnChance))
                {
                    if (Random.Range(0f, 1f) > 0.5f)
                    {
                        candidateTiles[baseTile.spawnChance] = baseTile;
                    }

                    spawnChance = baseTile.spawnChance + Random.Range(0f, 1f);
                    goto RECHECK;
                }

                candidateTiles.Add(spawnChance, baseTile);
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