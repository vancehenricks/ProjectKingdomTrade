/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public Color color;
    public HashSet<TileInfo> claims;
    public string playerName;
    public long playerId;

    public void Initialize()
    {
        playerId = Tools.UniqueId;
        name += "_" + playerId;
        claims = new HashSet<TileInfo>();
    }
}
