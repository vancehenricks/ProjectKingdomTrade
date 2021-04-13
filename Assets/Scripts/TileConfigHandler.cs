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
    public static TileConfigHandler init;
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
                Debug.LogError(e);
                ShowMessageHandler.init.infoWindow.SetMessage("Json Error - " + file,
                    e.ToString(), "OK", null, null);
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
                Debug.LogError(e);
                ShowMessageHandler.init.infoWindow.SetMessage("Json Error - " + file,
                    e.ToString(), "OK", null, null);
            }
        }
    }

    public TileConfig Convert(TileInfo tileInfo)
    {
        TileConfig config = new TileConfig();

        if (tileInfo.tileType == "Unit")
        {
            UnitInfo unitInfo = (UnitInfo)tileInfo;
            config.tileType = unitInfo.tileType;
            config.subType = unitInfo.subType;
            config.sprite = unitInfo.sprite.name;
            config.units = unitInfo.units;
            config.travelSpeed = unitInfo.travelSpeed;
            config.attackDistance = unitInfo.attackDistance;
            config.killChance = unitInfo.killChance;
            config.deathChance = unitInfo.deathChance;
            config.options = unitInfo.options.ToArray();
            config.nonWalkable = unitInfo.nonWalkable.ToArray();
        }
        else if (tileInfo.tileType != "Town")
        {
            config.tileType = tileInfo.tileType;
            config.subType = tileInfo.subType;
            config.travelTime = tileInfo.travelTime;
            config.minChance = tileInfo.minChance;
            config.maxChance = tileInfo.maxChance;

            config.sprite = tileInfo.tileEffect.image.sprite.name;
            config.freezingSprite = tileInfo.tileEffect.freezingTile.name;
            config.autumnSprite = tileInfo.tileEffect.autumnTile.name;
            config.summerSprite = tileInfo.tileEffect.summerTile.name;
            config.freezingTemp = tileInfo.tileEffect.freezingTemp;
            config.autumnTemp = tileInfo.tileEffect.autumnTemp;
            config.summerTemp = tileInfo.tileEffect.summerTemp;
            config.options = tileInfo.options.ToArray();
            config.isTownAllowed = tileInfo.isTownAllowed;
        }
        else if (tileInfo.tileType == "Town")
        {
            UnitInfo unitInfo = (UnitInfo)tileInfo;
            config.tileType = unitInfo.tileType;
            config.subType = unitInfo.subType;
            config.units = unitInfo.units;
            //config.travelSpeed = unitInfo.travelSpeed;
            config.attackDistance = unitInfo.attackDistance;
            config.killChance = unitInfo.killChance;
            config.deathChance = unitInfo.deathChance;
            config.options = unitInfo.options.ToArray();
            config.travelTime = unitInfo.travelTime;
            //config.minChance = unitInfo.minChance;
            //config.maxChance = unitInfo.maxChance;

            config.sprite = unitInfo.tileEffect.image.sprite.name;
            config.freezingSprite = unitInfo.tileEffect.freezingTile.name;
            config.autumnSprite = unitInfo.tileEffect.autumnTile.name;
            config.summerSprite = unitInfo.tileEffect.summerTile.name;
            config.freezingTemp = unitInfo.tileEffect.freezingTemp;
            config.autumnTemp = unitInfo.tileEffect.autumnTemp;
            config.summerTemp = unitInfo.tileEffect.summerTemp;
            config.options = unitInfo.options.ToArray();
        }

        return config;
    }


    public TileInfo Convert(TileConfig config)
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

            if (config.options != null)
            {
                unitInfo.options = new List<string>(config.options);
            }

            if (config.nonWalkable != null)
            {
                unitInfo.nonWalkable = new List<Walkable>(config.nonWalkable);
            }

            return unitInfo;
        }
        else if (config.tileType != "Town")
        {
            TileInfo tileInfo = Instantiate(baseTile);
            tileInfo.transform.SetParent(baseTile.transform.parent);
            tileInfo.tileType = config.tileType;
            tileInfo.subType = config.subType;
            tileInfo.travelTime = config.travelTime;
            tileInfo.minChance = config.minChance;
            tileInfo.maxChance = config.maxChance;

            if (config.options != null)
            {
                tileInfo.options = new List<string>(config.options);
            }

            Sprite sprite = TextureHandler.init.GetSprite(config.sprite);
            tileInfo.tileEffect.image.sprite = sprite;
            tileInfo.tileEffect.springTile = sprite;
            tileInfo.tileEffect.freezingTile = TextureHandler.init.GetSprite(config.freezingSprite);
            tileInfo.tileEffect.autumnTile = TextureHandler.init.GetSprite(config.autumnSprite);
            tileInfo.tileEffect.summerTile = TextureHandler.init.GetSprite(config.summerSprite);
            tileInfo.tileEffect.freezingTemp = config.freezingTemp;
            tileInfo.tileEffect.autumnTemp = config.autumnTemp;
            tileInfo.tileEffect.summerTemp = config.summerTemp;
            tileInfo.isTownAllowed = config.isTownAllowed;

            return tileInfo;
        }
        else if (config.tileType == "Town")
        {
            UnitInfo unitInfo = Instantiate(baseTown);
            unitInfo.transform.SetParent(baseUnit.transform.parent);
            unitInfo.tileType = config.tileType;
            unitInfo.subType = config.subType;
            unitInfo.units = config.units;
            //unitInfo.travelSpeed = config.travelSpeed;
            unitInfo.attackDistance = config.attackDistance;
            unitInfo.killChance = config.killChance;
            unitInfo.deathChance = config.deathChance;
            unitInfo.travelTime = config.travelTime;
            //unitInfo.minChance = config.minChance;
            //unitInfo.maxChance = config.maxChance;

            Sprite sprite = TextureHandler.init.GetSprite(config.sprite);
            //unitInfo.sprite = sprite;
            unitInfo.tileEffect.image.sprite = sprite;
            unitInfo.tileEffect.springTile = sprite;
            unitInfo.tileEffect.freezingTile = TextureHandler.init.GetSprite(config.freezingSprite);
            unitInfo.tileEffect.autumnTile = TextureHandler.init.GetSprite(config.autumnSprite);
            unitInfo.tileEffect.summerTile = TextureHandler.init.GetSprite(config.summerSprite);
            unitInfo.tileEffect.freezingTemp = config.freezingTemp;
            unitInfo.tileEffect.autumnTemp = config.autumnTemp;
            unitInfo.tileEffect.summerTemp = config.summerTemp;

            if (config.options != null)
            {
                unitInfo.options = new List<string>(config.options);
            }

            if (config.nonWalkable != null)
            {
                unitInfo.nonWalkable = new List<Walkable>(config.nonWalkable);
            }


            return unitInfo;
        }

        return null;
    }
}
