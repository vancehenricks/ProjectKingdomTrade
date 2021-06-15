﻿/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, June 2020
 */


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatHandler : MonoBehaviour
{
    public UnitInfo unitInfo;
    public PathFindingHandler pathFinding;
    //public delegate void TargetCountChange(TileInfo tile);
    //public TargetCountChange targetCountChange;
    //public delegate void FirstTargetChange(TileInfo tile);
    //public FirstTargetChange firstTargetChange;
    public TileInfoRaycaster tileInfoRaycaster;
    public CombatSession combatSession;
    private CombatSession defaultCombatSession;
    //private int previousTargetCount;
    private TileInfo firstTarget;
    private TileInfo targetStandingTile;
    public int targetIndex;
    
    private void Start()
    {
        defaultCombatSession = combatSession;
        Tick.init.tickUpdate += TickUpdate;
        firstTarget = unitInfo;
    }

    private void OnDestroy()
    {
        //UnitInfo targetUnit = (UnitInfo)unitInfo.currentTarget;

        //DisEngage(targetUnit);
        DisEngage();
        Tick.init.tickUpdate -= TickUpdate;
        //targetCountChange = null;
        //firstTargetChange = null;
    }

    private void TickUpdate()
    {
        RetrieveTarget();
        GenerateWaypoint(null, true);
    }

    private void RetrieveTarget()
    {
        if(unitInfo.currentTarget != null || unitInfo.targets.Count == 0) return;

        UnitInfo target = unitInfo.targets[targetIndex] as UnitInfo;

        if (target == null)
        {
            unitInfo.targets.RemoveAt(targetIndex);
            ResetCombatPathing();
            GenerateWaypoint(null, true);
            return;
        }

        //create an instance of CombatSession inside target
        unitInfo.currentTarget = target;
        AddToIndex(target.targetted, 0);

        //Issue with this executing the last target since there is no checking here ideal targetted unit will delete itself in unitInfo.targets
        //This will be catched on target == null
        //combatSession.ClearCombantants();
        target.unitEffect.combatHandler.combatSession.Add(target);
        combatSession = target.unitEffect.combatHandler.combatSession;
        combatSession.Add(unitInfo);
        combatSession.Relay();


        if(targetIndex < unitInfo.targets.Count)
        {
            targetIndex++;
        }
        else
        {
            targetIndex=0;
        }
    }

    public void GenerateWaypoint(TileInfo waypoint, bool checkOnlyWithinDistance = false)
    {
        if (unitInfo.currentTarget == null)
        {
            if (unitInfo.targets.Count > 0)
            {
                ResetCombatPathing();
            }

            return;
        }

        UnitInfo targetUnit = (UnitInfo)unitInfo.currentTarget;

        int distance = Tools.TileLocationDistance(unitInfo, targetUnit);
        int attackDistance = unitInfo.attackDistance <= 1 ? 0 : unitInfo.attackDistance;

        CDebug.Log(this,"unitInfo.tileId=" + unitInfo.tileId + " distance=" + distance);

        if (distance <= attackDistance && !unitInfo.isEngaged)
        {
            CDebug.Log(this,"unitInfo.tileId=" + unitInfo.tileId + " distance=" + distance);

            AddToIndex(targetUnit.targets, 0);
            targetUnit.currentTarget = unitInfo;

            unitInfo.isEngaged = true;
            targetStandingTile = null;
            ResetCombatPathing();
            targetUnit.waypoints.Add(unitInfo);
        }
        else if (distance <= attackDistance && unitInfo.isEngaged)
        {
            //damage enemy logic here
            CDebug.Log(this, "unitInfo.tileId=" + unitInfo.tileId + " targetUnit.tileId=" + targetUnit.tileId, LogType.Warning);
        }
        else if (distance > attackDistance && unitInfo.isEngaged)
        {
            DisEngage();
        }
        else if (distance > attackDistance && !checkOnlyWithinDistance)
        {
            ResetCombatPathing();
            unitInfo.waypoints.Add(waypoint);
            targetStandingTile = targetUnit.standingTile;
        }
        else if (!unitInfo.isEngaged &&
            distance > attackDistance &&
            pathFinding.gwPointsIndex >= pathFinding.generatedWayPoints.Count/2)
        {
            if (targetStandingTile == null) return;
            if (targetUnit.standingTile != null && targetUnit.standingTile.tileId == targetStandingTile.tileId) return;
            if (targetUnit.currentTarget != null && targetUnit.currentTarget.tileId == unitInfo.tileId) return;

            ResetCombatPathing();
            unitInfo.waypoints.Add(targetUnit);
            targetStandingTile = targetUnit.standingTile;
        }
    }

    private void ResetCombatPathing()
    {
        pathFinding.ResetDestination();
        pathFinding.ResetGeneratedWaypoints();
        unitInfo.waypoints.Clear();
    }

    private void AddToIndex(List<TileInfo> tileInfos, int index)
    {
        if (Tools.Exist<TileInfo>(tileInfos, unitInfo) > -1)
        {
            tileInfos.Remove(unitInfo);
            tileInfos.Insert(index, unitInfo);
        }
        else
        {
            tileInfos.Add(unitInfo);
        }
    }

    public void DisEngage()
    {
        CDebug.Log(this, " unitInfo.tileId=" + unitInfo.tileId + " Disengaging!", LogType.Warning);

        if (unitInfo.isEngaged || unitInfo.currentTarget != null)
        {
            if (combatSession != null && defaultCombatSession != null)
            {
                combatSession.Remove(unitInfo);
                combatSession = defaultCombatSession;
            }
            unitInfo.targetted.Remove(unitInfo.currentTarget);
            unitInfo.targets.Remove(unitInfo.currentTarget);
            unitInfo.currentTarget = null;
            unitInfo.isEngaged = false;
        }

        ResetCombatPathing();
    }
}
