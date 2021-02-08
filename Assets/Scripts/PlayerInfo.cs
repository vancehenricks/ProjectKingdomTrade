/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2020
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
        claims = new HashSet<TileInfo>();
    }
}
