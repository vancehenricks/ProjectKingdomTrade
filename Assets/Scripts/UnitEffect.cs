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
    //public UnitOcclusion unitOcclusion;
    public TileCollider territoryCollider;

    public override void Initialize()
    {
        if(territoryCollider != null)
        {
            territoryCollider.onEnter += OnEnterTerritory;
            territoryCollider.onExit += OnExitTerritory;
            territoryCollider.Initialize(true);
            territoryCollider.Listen();
        }

        base.Initialize();
    }

    protected override void OnDestroy()
    {
        if (unitInfo.standingTile != null)
        {
            unitInfo.standingTile.standingTiles.Remove(unitInfo);
            ResetDisplay(unitInfo.standingTile);
        }

        if(territoryCollider != null)
        {
            territoryCollider.onEnter -= OnEnterTerritory;
            territoryCollider.onExit -= OnExitTerritory;
        }

        base.OnDestroy();
    }

    protected virtual void OnEnterTerritory(List<BaseInfo> baseInfos)
    {
        foreach(BaseInfo baseInfo in baseInfos)
        {
            TileInfo tileInfo = baseInfo as TileInfo;

            if(tileInfo != null)
            {
                tileInfo.tileEffect.UpdateTerritory(unitInfo);
            }
        }
    }   

    protected virtual void OnExitTerritory(List<BaseInfo> baseInfos)
    {

    }    

    protected override void OnEnter(List<BaseInfo> baseInfos)
    {
        if (unitInfo.tileType == "Town") return;

        //CDebug.Log(nameof(UnitEffect), "tileInfos.Count= " + tileInfos.Count, LogType.Warning);

        foreach(TileInfo tile in baseInfos)
        {
            //CDebug.Log(nameof(UnitEffect), "tile.standingTiles.Count=" + tile.standingTiles.Count, LogType.Warning);

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

    protected override void OnExit(List<BaseInfo> baseInfos)
    {
        if (unitInfo.tileType == "Town") return;

        foreach(TileInfo tile in baseInfos)
        {
            if (tile.tileType != unitInfo.tileType && tile.standingTiles.Contains(unitInfo))
            {
                tile.standingTiles.Remove(unitInfo);
                ResetDisplay(tile);
            }
        }

        //transform.SetParent(MapGenerator.init.grid);
    }

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
