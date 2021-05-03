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

    public TileInfo standingTile;
    public PathFindingHandler pathFinder;
    public CombatHandler combatHandler;
    public MergeHandler mergeHandler;
    public UnitDirection unitDirection;
    public UnitDisplay unitDisplay;
    public UnitWayPoint unitWayPoint;
    public NonWalkableTiles nonWalkableTiles;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (unitInfo.tileType == "Town") return;

        TileInfo tempTile = collision.GetComponent<TileInfo>();

        if (tempTile == null)
        {
            return;
        }

        if (tempTile.tileType != unitInfo.tileType)
        {
            standingTile = tempTile;
        }

        unitInfo.tileLocation = tempTile.tileLocation;
        unitInfo.localTemp = tempTile.localTemp;
        unitInfo.travelTime = tempTile.travelTime;
    }
}
