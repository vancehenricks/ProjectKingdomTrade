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

    private void OnDestroy()
    {
        if (unitInfo.standingTile != null)
        {
            unitInfo.standingTile.standingTiles.Remove(unitInfo);
            ResetDisplay(unitInfo.standingTile);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (unitInfo.tileType == "Town") return;

        TileInfo tempTile = collision.GetComponent<TileInfo>();

        if (tempTile == null) return;

        if (tempTile.tileType != unitInfo.tileType)
        {
            transform.SetParent(tempTile.transform.parent);
            tempTile.standingTiles.Add(unitInfo);
            ResetDisplay(tempTile);
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
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
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (unitInfo.tileType == "Town") return;

        TileInfo tempTile = collision.GetComponent<TileInfo>();

        if (tempTile == null) return;

        if (tempTile.tileType != unitInfo.tileType)
        {
            tempTile.standingTiles.Remove(unitInfo);
            ResetDisplay(tempTile);
        }

        //transform.SetParent(MapGenerator.init.grid);
    }


    public void ResetDisplay(TileInfo standingTile)
    {
        standingTile.tileEffect.unitDisplay.Sync();
        foreach (TileInfo tile in standingTile.standingTiles)
        {
            tile.tileEffect.unitDisplay.Sync();
        }
    }
}
