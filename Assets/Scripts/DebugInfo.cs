﻿/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, August 2019
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugInfo : MonoBehaviour
{
    public Text fps;
    public Text ms;
    public Text playerName;
    public Text playerId;
    public Text tileId;
    public Text subType;
    public Text tileType;
    public Text tileLocation;

    private void Start()
    {
        MultiSelect.onSelectedChange += OnSelectedChange;
    }

    public void OnSelectedChange(List<TileInfo> tiles)
    {
        if (tiles == null || tiles.Count == 0) return;

        int lastIndex = tiles.Count - 1;
        TileInfo tile = tiles[lastIndex];
        PlayerInfo playerInfo = tile.playerInfo;

        tileId.text = tile.tileId + "";
        subType.text = tile.subType + "";
        tileType.text = tile.tileType + "";
        tileLocation.text = tile.tileLocation + "";

        if (playerInfo == null)
        {
            playerId.text = "NULL";
            playerName.text = "NULL";
        }
        else
        {
            playerId.text = playerInfo.playerId + "";
            playerName.text = playerInfo.playerName + "";
        }
    }

    private void Update()
    {
        ms.text = string.Format("{0:0.##}", Time.deltaTime * 1000.0f);
        fps.text = string.Format("{0:0}", 1.0f / Time.deltaTime);
    }
}
