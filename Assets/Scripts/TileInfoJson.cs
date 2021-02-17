/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2021
 */

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TileInfoJson
{
    public string baseSprite;
    public string freezingSprite;
    public string autumnSprite;
    public string summerSprite;
    public string tileType;
    public string subType;
    public long tileId;
    public string tileName;
    //public PlayerInfo playerInfo;
    //public HashSet<PlayerInfo> claimants;
    //public TileEffect tileEffect;
    //public TileCaller tileCaller;
    public Vector2 tileLocation;
    public float localTemp;
    public float travelTime;
    public int minChance;
    public int maxChance;
    //public List<UnitInfo> unitInfos;
    public int units;
    //public bool selected;
    public float travelSpeed;
    public bool isEngaged;
    public int attackDistance;
    public float killChance;
    public float deathChance;
}