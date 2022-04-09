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


///<summary>
///Generate TileInfo from config files<br/>
///</summary>
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

    [SerializeField]
    private TileInfo baseTile;

    [SerializeField]
    private UnitInfo baseTown;

    [SerializeField]
    private UnitInfo baseUnit;

    private void Awake()
    {
        init = this;
    }

    ///<summary>
    ///Load config files and convert to TileInfo.<br/>
    ///</summary>
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
                CDebug.Log(this,e,LogType.Error);
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
                CDebug.Log(this,e,LogType.Error);
                ShowMessageHandler.init.infoWindow.SetMessage("Json [ERROR] - " + file,
                    e.ToString(), "[OK]", null, null);
            }
        }
    }

    ///<summary>
    ///Convert TileInfo to TileConfig<br/>
    ///Returns TileConfig.<br/>
    ///</summary>
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
            config.spawnTime = unitInfo.spawnTime;
            config.unitSpawnable = unitInfo.unitSpawnable.ToArray();
        }

        config.unit = tileInfo.unit;
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
        config.upgrades = tileInfo.upgrades.ToArray();
        config.spawnDistance = tileInfo.spawnDistance.ToArray();
        config.isPlayer = tileInfo.isPlayer;
        config.spawnLayer = tileInfo.spawnLayer;
        config.cloudDrag = tileInfo.cloudDrag;
        config.dragAffectedByCloud = tileInfo.dragAffectedByCloud;

        return config;
    }

    ///<summary>
    ///Convert TileConfig to TileInfo<br/>
    ///Returns TileInfo.<br/>
    ///</summary>
    public TileInfo Convert(TileConfig config)
    {
        UnityEngine.Object baseObj = baseTile;

        if (config.tileType == "Unit")
        {
            baseObj = baseUnit;
        }
        else if (config.tileType == "Town")
        {
            baseObj = baseTown;
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
            unitInfo.spawnTime = config.spawnTime;

            if (config.nonWalkable != null)
            {
                unitInfo.nonWalkable = new List<Walkable>(config.nonWalkable);
            }

            if(config.unitSpawnable != null)
            {
                unitInfo.unitSpawnable = new List<string>(config.unitSpawnable);
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

        if (config.spawnDistance != null)
        {
            tileInfo.spawnDistance = new List<SpawnDistance>(config.spawnDistance);
        }

        if (config.upgrades != null)
        {
            tileInfo.upgrades = new List<Upgrade>(config.upgrades);

            foreach (Upgrade upgrade in config.upgrades)
            {
                TextureHandler.init.GetSprite(upgrade.spriteReward); //Load sprite into cache
            }
        }

        tileInfo.unit = config.unit;
        tileInfo.tileType = config.tileType;
        tileInfo.subType = config.subType;
        tileInfo.travelTime = config.travelTime;
        tileInfo.spawnHeightMin = config.spawnHeightMin;
        tileInfo.spawnHeightMax = config.spawnHeightMax;
        tileInfo.spawnChance = config.spawnChance;
        tileInfo.isPlayer = config.isPlayer;
        tileInfo.spawnLayer = config.spawnLayer;
        tileInfo.cloudDrag = config.cloudDrag;
        tileInfo.dragAffectedByCloud = config.dragAffectedByCloud;

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

        //finalize object name for easy searching
        if(tileInfo.subType == "")
        {
            tileInfo.name = baseObj.name + "_" + tileInfo.tileType;
        }
        else
        {
            tileInfo.name = baseObj.name + "_" + tileInfo.tileType + "_" + tileInfo.subType;
        }

        return tileInfo;
    }

    ///<summary>
    ///Returns the base tile of given sub-type.<br/>
    ///Returns TileInfo.<br/>
    ///</summary>
    public TileInfo Serialize(string name)
    {
        if(init.baseUnits.ContainsKey(name))
        {
            return init.baseUnits[name];
        }

        if(init.baseTiles.ContainsKey(name))
        {
            return init.baseTiles[name];
        }

        if(init.baseTowns.ContainsKey(name))
        {
            return init.baseTowns[name];
        }

        return null;
    }    
}
