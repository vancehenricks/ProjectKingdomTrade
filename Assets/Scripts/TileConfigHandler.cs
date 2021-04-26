/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2021
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TileConfigHandler : MonoBehaviour
{
    public Dictionary<string, TileInfo> baseTiles;
    public Dictionary<string, TileInfo> baseUnits;
    public Dictionary<string, TileInfo> baseTowns;
    private static TileConfigHandler _init;
    public static TileConfigHandler init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public TileInfo baseTile;
    public UnitInfo baseTown;
    public UnitInfo baseUnit;

    private void Awake()
    {
        init = this;
    }

    public void Load()
    {
        baseTiles = new Dictionary<string, TileInfo>();
        baseUnits = new Dictionary<string, TileInfo>();
        baseTowns = new Dictionary<string, TileInfo>();

        List<string> files = new List<string>();
        string[] includePaths = Directory.GetFiles(Path.Combine(Application.streamingAssetsPath, "Config"), "*include");

        foreach (string includePath in includePaths)
        {
            string[] temp = File.ReadAllLines(includePath);
            files.AddRange(temp);
        }

        string tilePath = Path.Combine(Application.streamingAssetsPath, "Config", "Tiles");

        foreach (string file in files)
        {

            if (file.Contains("//")) continue;

            TileInfo tileInfo = null;

            string json = File.ReadAllText(Path.Combine(tilePath, file));
            try
            {
                tileInfo = Convert(JsonUtility.FromJson<TileConfig>(json));
                Tools.WriteTileConfig(Convert(tileInfo), file);
            }
            catch (System.Exception e)
            {
                Tools.Log(this,e,LogType.Error);
                ShowMessageHandler.init.infoWindow.SetMessage("Json [ERROR] - " + file,
                    e.ToString(), "[OK]", null, null);
            }

            try
            {
                if (tileInfo.tileType == "Town")
                {
                    baseTowns.Add(tileInfo.subType, tileInfo);
                }
                else if (tileInfo.subType == "")
                {
                    baseTiles.Add(tileInfo.tileType, tileInfo);
                }
                else if (tileInfo.tileType == "Unit")
                {
                    baseUnits.Add(tileInfo.subType, tileInfo);
                }
            }
            catch (System.Exception e)
            {
                Tools.Log(this,e,LogType.Error);
                ShowMessageHandler.init.infoWindow.SetMessage("Json [ERROR] - " + file,
                    e.ToString(), "[OK]", null, null);
            }
        }
    }

    public TileConfig Convert(TileInfo tileInfo)
    {
        TileConfig config = new TileConfig();
        UnitInfo unitInfo = tileInfo as UnitInfo;

        if (unitInfo != null)
        {
            config.travelSpeed = unitInfo.travelSpeed;
            config.attackDistance = unitInfo.attackDistance;
            config.killChance = unitInfo.killChance;
            config.deathChance = unitInfo.deathChance;
            config.nonWalkable = unitInfo.nonWalkable.ToArray();
        }

        config.units = tileInfo.units;
        config.tileType = tileInfo.tileType;
        config.subType = tileInfo.subType;
        config.travelTime = tileInfo.travelTime;
        config.spawnHeightMin = tileInfo.spawnHeightMin;
        config.spawnHeightMax = tileInfo.spawnHeightMax;
        config.spawnChance = tileInfo.spawnChance;
        config.spawnableTile = tileInfo.spawnableTile.ToArray();

        config.sprite = tileInfo.sprite.name;

        if (tileInfo.tileEffect.freezingTile != null)
        {
            config.freezingSprite = tileInfo.tileEffect.freezingTile.name;
        }

        if (tileInfo.tileEffect.autumnTile != null)
        {
            config.autumnSprite = tileInfo.tileEffect.autumnTile.name;
        }

        if (tileInfo.tileEffect.summerTile != null)
        {
            config.summerSprite = tileInfo.tileEffect.summerTile.name;
        }

        config.freezingTemp = tileInfo.tileEffect.freezingTemp;
        config.autumnTemp = tileInfo.tileEffect.autumnTemp;
        config.summerTemp = tileInfo.tileEffect.summerTemp;
        config.options = tileInfo.options.ToArray();

        return config;
    }


    public TileInfo Convert(TileConfig config)
    {
        UnityEngine.Object baseObj = baseTile;

        if (config.tileType == "Unit" || config.tileType == "Town")
        {
            baseObj = baseUnit;
        }

        TileInfo tileInfo = (TileInfo)Instantiate(baseObj);
        UnitInfo unitInfo = tileInfo as UnitInfo;

        tileInfo.transform.SetParent(baseUnit.transform.parent);

        if (unitInfo != null)
        {
            unitInfo.travelSpeed = config.travelSpeed;
            unitInfo.attackDistance = config.attackDistance;
            unitInfo.killChance = config.killChance;
            unitInfo.deathChance = config.deathChance;

            if (config.nonWalkable != null)
            {
                unitInfo.nonWalkable = new List<Walkable>(config.nonWalkable);
            }
        }

        if (config.spawnableTile != null)
        {
            tileInfo.spawnableTile = new List<string>(config.spawnableTile);
        }

        if (config.options != null)
        {
            tileInfo.options = new List<string>(config.options);
        }

        tileInfo.units = config.units;
        tileInfo.tileType = config.tileType;
        tileInfo.subType = config.subType;
        tileInfo.travelTime = config.travelTime;
        tileInfo.spawnHeightMin = config.spawnHeightMin;
        tileInfo.spawnHeightMax = config.spawnHeightMax;
        tileInfo.spawnChance = config.spawnChance;

        Sprite sprite = TextureHandler.init.GetSprite(config.sprite);

        if (sprite != null)
        {
            tileInfo.sprite = sprite;
            tileInfo.tileEffect.springTile = sprite;
        }

        tileInfo.tileEffect.freezingTile = TextureHandler.init.GetSprite(config.freezingSprite);
        tileInfo.tileEffect.autumnTile = TextureHandler.init.GetSprite(config.autumnSprite);
        tileInfo.tileEffect.summerTile = TextureHandler.init.GetSprite(config.summerSprite);
        tileInfo.tileEffect.freezingTemp = config.freezingTemp;
        tileInfo.tileEffect.autumnTemp = config.autumnTemp;
        tileInfo.tileEffect.summerTemp = config.summerTemp;

        return tileInfo;
    }
}
