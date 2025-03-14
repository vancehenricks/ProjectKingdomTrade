﻿/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, January 2019
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileInfo : BaseInfo
{
    [SerializeField]
    protected Sprite _sprite;
    public Sprite sprite
    {
        get
        {
            return GetSprite();
        }
        set
        {
            SetSprite(value);
            _sprite = value;
        }
    }

    //public string tileType;
    //public string subType;
    //public long tileId;
    public string tileName;
    public PlayerInfo playerInfo;
    public HashSet<PlayerInfo> claimants;
    public TileEffect tileEffect;
    //Tilelocation already has tile size accomodated to it no need for dividing 25 again
    //public Vector2Int tileLocation;
    public float localTemp;
    public float travelTime; //seconds
    public bool isPlayer;
    public int spawnLayer;
    public float spawnHeightMin;
    public float spawnHeightMax;
    public float spawnChance;
    public List<SpawnDistance> spawnDistance;
    public List<string> spawnableTile;
    public int unit;
    public int maxUnit;
    public bool selected;
    public float cloudDrag;
    public float dragAffectedByCloud;
    //public float spawnTime;
    public List<string> options;
    public List<TileInfo> standingTiles;
    public List<Upgrade> upgrades;
    public Dictionary<string,Queue<RecruitInfo>> recruitInfos;
    //public List<string> spawnables;
    //public TileCollider tileCollider;

    protected virtual void Awake()
    {
        claimants = new HashSet<PlayerInfo>();
        recruitInfos = new Dictionary<string,Queue<RecruitInfo>>();
    }

    public override void Initialize()
    {
        base.Initialize();

        if (playerInfo == null)
        {
            playerInfo = PlayerList.init.defaultPlayerInfo;
        }

        name += "_" + playerInfo.playerId;
        localTemp = Temperature.init.temperature;
        //unitInfos = new List<UnitInfo>();
        //upgrades = new List<Upgrade>();
        //spawnDistance = new List<SpawnDistance>();
        //gameObject.name = gameObject.name + "_" + tileId;
        TileList.init.Add(this);
        localTemp = Temperature.init.temperature;
        //SetSprite(sprite);
        tileEffect.Initialize();
    }

    public virtual void OnDestroy()
    {
        if (claimants != null) claimants.Clear();
        //TileList.init.Remove(this); this causes timing issue with dictionary moved to UnitInfo instead
    }

    public void Destroy()
    {
        TileList.init.Remove(this);
        Destroy(gameObject);
    }

    protected virtual void SetSprite(Sprite sp)
    {
        if (sp != null)
        {
            tileEffect.imageImage.sprite = sp;
            _sprite = sp;
        }
    }

    protected virtual Sprite GetSprite()
    {
        return _sprite;
    } 
}
