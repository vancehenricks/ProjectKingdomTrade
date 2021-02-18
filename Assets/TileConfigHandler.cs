/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2021
 */

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TileConfigHandler : MonoBehaviour
{
    public Dictionary<string, TileInfo> baseTiles;
    public static TileConfigHandler init;
    public TileInfo baseTile;
    public UnitInfo baseUnit;

    private void Awake()
    {
        init = this;
    }

    public void LoadTiles()
    {
        baseTiles = new Dictionary<string, TileInfo>();
        string includePath = Path.Combine(Application.streamingAssetsPath, "Config/include");
        string[] files = File.ReadAllLines(includePath);

        string tilePath = Path.Combine(Application.streamingAssetsPath, "Config/Tiles/");

        float progress = 0f;
        float count = 0;
        float total = files.Length;

        foreach (string file in files)
        {
            string json = File.ReadAllText(tilePath + file);
            TileInfo tileInfo = GenerateTileFromConfig(JsonUtility.FromJson<TileConfig>(json));

            if (tileInfo.subType == "")
            {
                baseTiles.Add(tileInfo.tileType, tileInfo);
            }
            else
            {
                baseTiles.Add(tileInfo.subType, tileInfo);
            }
            count++;
            progress = count / total;
        }
    }

    public TileInfo GenerateTileFromConfig(TileConfig config)
    {
        if (config.tileType == "Unit")
        {
            UnitInfo unitInfo = Instantiate(baseUnit);
            unitInfo.transform.SetParent(baseUnit.transform.parent);
            unitInfo.tileType = config.tileType;
            unitInfo.subType = config.subType;
            unitInfo.sprite = TextureHandler.init.GetSprite(config.sprite);
            unitInfo.units = config.units;
            unitInfo.travelSpeed = config.travelSpeed;
            unitInfo.attackDistance = config.attackDistance;
            unitInfo.killChance = config.killChance;
            unitInfo.deathChance = config.deathChance;

            return unitInfo;
        }
        else if (config.tileType != "Town")
        {
            TileInfo tileInfo = Instantiate(baseTile);
            tileInfo.transform.SetParent(baseTile.transform.parent);
            tileInfo.tileType = config.tileType;
            tileInfo.subType = config.subType;
            Sprite sprite = TextureHandler.init.GetSprite(config.sprite);
            tileInfo.tileEffect.image.sprite = sprite;
            tileInfo.tileEffect.springTile = sprite;
            tileInfo.tileEffect.freezingTile = TextureHandler.init.GetSprite(config.freezingSprite);
            tileInfo.tileEffect.autumnTile = TextureHandler.init.GetSprite(config.autumnSprite);
            tileInfo.tileEffect.summerTile = TextureHandler.init.GetSprite(config.summerSprite);

            return tileInfo;
        }

        return null;
    }
}
