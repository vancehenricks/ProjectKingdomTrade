/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, January 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfo : MonoBehaviour
{
    public string tileType;
    public string subType;
    public long tileId;
    public string tileName;
    public PlayerInfo playerInfo;
    public HashSet<PlayerInfo> claimants;
    public TileEffect tileEffect;
    public TileCaller tileCaller;
    public Color color;
    //Tilelocation already has tile size accomodated to it no need for dividing 25 again
    public Vector2 tileLocation;
    public float localTemp;
    public float travelTime; //seconds
    public int minChance;
    public int maxChance;
    public List<UnitInfo> unitInfos;
    public int units;
    public bool selected;

    public virtual void Initialize()
    {
        claimants = new HashSet<PlayerInfo>();
        localTemp = Temperature.temperature;
        unitInfos = new List<UnitInfo>();
        tileId = Tools.UniqueId;
    }

    public void OnDestroy()
    {
        if (unitInfos != null) unitInfos.Clear();
        if (claimants != null) claimants.Clear();
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
