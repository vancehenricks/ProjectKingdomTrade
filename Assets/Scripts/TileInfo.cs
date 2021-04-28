/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, January 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileInfo : MonoBehaviour
{
    protected Sprite _sprite;
    public Sprite sprite
    {
        get
        {
            return GetSprite();
        }
        set
        {
            _sprite = SetSprite(value);
        }
    }

    public string tileType;
    public string subType;
    public long tileId;
    public string tileName;
    public PlayerInfo playerInfo;
    public HashSet<PlayerInfo> claimants;
    public TileEffect tileEffect;
    //Tilelocation already has tile size accomodated to it no need for dividing 25 again
    public Vector2 tileLocation;
    public float localTemp;
    public float travelTime; //seconds
    public float spawnHeightMin;
    public float spawnHeightMax;
    public float spawnChance;
    public float spawnDistance;
    public List<string> spawnableTile;
    public List<UnitInfo> unitInfos;
    public int unit;
    public int maxUnit;
    public bool selected;
    public List<string> options;
    public List<Upgrade> upgrades;

    public virtual void Initialize()
    {
        if (playerInfo == null)
        {
            playerInfo = PlayerList.init.defaultPlayerInfo;
        }
        claimants = new HashSet<PlayerInfo>();
        localTemp = Temperature.init.temperature;
        unitInfos = new List<UnitInfo>();
        upgrades = new List<Upgrade>();
        tileId = Tools.UniqueId;
        TileList.init.Add(this);

        SetSprite(sprite);
    }

    public void OnDestroy()
    {
        if (unitInfos != null) unitInfos.Clear();
        if (claimants != null) claimants.Clear();
        TileList.init.Remove(this);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    protected virtual Sprite SetSprite(Sprite sp)
    {
        if (sp != null)
        {
            tileEffect.imageImage.sprite = sp;
        }

        return sp;
    }

    protected virtual Sprite GetSprite()
    {
        return _sprite;
    }
}
