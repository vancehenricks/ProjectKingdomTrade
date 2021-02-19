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
    public Dictionary<string, TileInfo> baseUnits;
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
        baseUnits = new Dictionary<string, TileInfo>();

        string includePath = Path.Combine(Application.streamingAssetsPath, "Config/include");
        string[] files = File.ReadAllLines(includePath);

        string tilePath = Path.Combine(Application.streamingAssetsPath, "Config/Tiles/");

        foreach (string file in files)
        {

            if (file.Contains("//")) continue;

            TileInfo tileInfo = null;

            string json = File.ReadAllText(tilePath + file);
            try
            {
                tileInfo = Convert(JsonUtility.FromJson<TileConfig>(json));
                Tools.WriteTileConfig(Convert(tileInfo), file);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                ShowMessageHandler.init.infoWindow.SetMessage("Json Error",
                    "Please check " + file + " and correct any syntax errors.", "OK", null, null);
            }

            try
            {
                if (tileInfo.subType == "")
                {
                    baseTiles.Add(tileInfo.tileType, tileInfo);
                }
                else if(tileInfo.tileType == "Unit")
                {
                    baseUnits.Add(tileInfo.subType, tileInfo);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                ShowMessageHandler.init.infoWindow.SetMessage("Json Error",
                    "Please check " + file + " and make sure tileType is unique if subType " +
                    "is empty else make sure subType is unique.", "OK", null, null);
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
        }
        else if (config.tileType != "Town")
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
        }

        //Add town later

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


            Sprite sprite = TextureHandler.init.GetSprite(config.sprite);
            tileInfo.tileEffect.image.sprite = sprite;
            tileInfo.tileEffect.springTile = sprite;
            tileInfo.tileEffect.freezingTile = TextureHandler.init.GetSprite(config.freezingSprite);
            tileInfo.tileEffect.autumnTile = TextureHandler.init.GetSprite(config.autumnSprite);
            tileInfo.tileEffect.summerTile = TextureHandler.init.GetSprite(config.summerSprite);
            tileInfo.tileEffect.freezingTemp = config.freezingTemp;
            tileInfo.tileEffect.autumnTemp = config.autumnTemp;
            tileInfo.tileEffect.summerTemp = config.summerTemp;

            return tileInfo;
        }
        //Add for town

        return null;
    }
}
