/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, August 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitEffect : TileEffect
{
    private UnitInfo _unitInfo;
    public UnitInfo unitInfo
    {
        get
        {
            if (_unitInfo == null)
            {
                _unitInfo = (UnitInfo)tileInfo;
            }

            return _unitInfo;
        }
        private set
        {
            tileInfo = value;
        }
    }

    public PathFindingHandler pathFinder;
    public CombatHandler combatHandler;
    public MergeHandler mergeHandler;
    public UnitDirection unitDirection;
    public UnitWayPoint unitWayPoint;
    public NonWalkableTiles nonWalkableTiles;
    public UnitOcclusion unitOcclusion;

    protected override void OnDestroy()
    {
        if (unitInfo.standingTile != null)
        {
            unitInfo.standingTile.standingTiles.Remove(unitInfo);
            ResetDisplay(unitInfo.standingTile);
        }

        base.OnDestroy();
    }
    
    //Different thread+
    protected override void OnEnter(List<TileInfo> tileInfos)
    {
        if (unitInfo.tileType == "Town") return;

        foreach(TileInfo tile in tileInfos)
        {
            if (tile.tileType != unitInfo.tileType && !tile.standingTiles.Contains(unitInfo))
            {
                transform.SetParent(tile.transform.parent);
                tile.standingTiles.Add(unitInfo);
                ResetDisplay(tile);

                unitInfo.standingTile = tile;
                unitInfo.tileLocation = tile.tileLocation;
                unitInfo.travelTime = tile.travelTime;
                break;            
            }
        }
    }
    //Different thread-

    /*private void OnTriggerStay2D(Collider2D collision)
    {
        if (unitInfo.tileType == "Town") return;

        TileInfo tempTile = collision.GetComponent<TileInfo>();

        if (tempTile == null) return;

        if (tempTile.tileType != unitInfo.tileType)
        {
            unitInfo.standingTile = tempTile;
            unitInfo.tileLocation = tempTile.tileLocation;
            unitInfo.travelTime = tempTile.travelTime;
        }

        unitInfo.localTemp = tempTile.localTemp;
    }*/

    //Different thread+
    protected override void OnExit(List<TileInfo> tileInfos)
    {
        if (unitInfo.tileType == "Town") return;

        foreach(TileInfo tile in tileInfos)
        {
            if (tile.tileType != unitInfo.tileType && tile.standingTiles.Contains(unitInfo))
            {
                tile.standingTiles.Remove(unitInfo);
                ResetDisplay(tile);
            }
        }

        //transform.SetParent(MapGenerator.init.grid);
    }
    //Different thread-    


    public void ResetDisplay(TileInfo standingTile)
    {
        standingTile.tileEffect.unitDisplay.Sync();
        foreach (TileInfo tile in standingTile.standingTiles)
        {
            if (tile.selected)
            {
                tile.transform.SetAsLastSibling();
            }
            tile.tileEffect.unitDisplay.Sync();
        }
    }
}
