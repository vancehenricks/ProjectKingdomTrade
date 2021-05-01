/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
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
    public Text version;
    public Text fps;
    public Text ms;
    public Text playerName;
    public Text playerId;
    public Text tileId;
    public Text subType;
    public Text tileType;
    public Text tileLocation;
    public Text xOffset;
    public Text yOffset;
    public Text scale;
    public Text mapSize;

    private TileInfo tile;
    private PlayerInfo playerInfo;

    private bool initialize;

    private void Awake()
    {
        version.text = Application.version;
    }

    public void OnSelectedChange(List<TileInfo> tiles)
    {
        if (tiles == null || tiles.Count == 0) return;

        int lastIndex = tiles.Count - 1;

        tile = tiles[lastIndex];
        playerInfo = tile.playerInfo;
        tileId.text = tile.tileId + "";
        subType.text = tile.subType + "";
        tileType.text = tile.tileType + "";

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

        xOffset.text = MapGenerator.init.xOffset.ToString("0.00");
        yOffset.text = MapGenerator.init.yOffset.ToString("0.00");
        scale.text = MapGenerator.init.scale.ToString("0.00");
        mapSize.text = "(" + MapGenerator.init.width + ", " + MapGenerator.init.height + ")";
    }

    private void Update()
    {
        if (tile != null)
        {
            tileLocation.text = tile.tileLocation + "";
        }
        else if (!initialize && MultiSelect.init != null)
        {
            MultiSelect.init.onSelectedChange += OnSelectedChange;
            initialize = true;
        }

        ms.text = string.Format("{0:0.##}", Time.deltaTime * 1000.0f);
        fps.text = string.Format("{0:0}", 1.0f / Time.deltaTime);
    }
}
