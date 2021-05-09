/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, June 2020
 */

using DebugHandler;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatHandler : MonoBehaviour
{
    public UnitInfo unitInfo;
    public PathFindingHandler pathFinding;
    public delegate void TargetCountChange(TileInfo tile);
    public TargetCountChange targetCountChange;
    public delegate void FirstTargetChange(TileInfo tile);
    public FirstTargetChange firstTargetChange;
    public TileInfoRaycaster tileInfoRaycaster;
    public CombatSession combatSession;

    private CombatSession defaultCombatSession;

    private int previousTargetCount;
    private TileInfo firstTarget;
    private TileInfo targetStandingTile;

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
        targetCountChange = null;
        firstTargetChange = null;
    }

    private void Update()
    {
        if (unitInfo.currentTarget == null)
        {
            //Issue with selecting towns -- to be implemented later for attacking towns
            try
            {
                foreach (UnitInfo target in unitInfo.targets)
                {
                    //Debug.Log("41");

                    if (target == null)
                    {
                        unitInfo.targets.Remove(target);
                        ResetCombatPathing();
                        break;
                    }

                    //create an instance of CombatSession inside target
                    unitInfo.currentTarget = target;
                    AddToIndex(target.targetted, unitInfo, 0);

                    //Issue with this executing the last target since there is no checking here ideal targetted unit will delete itself in unitInfo.targets
                    //This will be catched on target == null
                    //combatSession.ClearCombantants();
                    target.unitEffect.combatHandler.combatSession.Add(target);
                    combatSession = target.unitEffect.combatHandler.combatSession;
                    combatSession.Add(unitInfo);
                    combatSession.Relay();
                    break;
                }
            }
            catch (System.Exception e)
            {
                CDebug.Log(this,e,LogType.Error);
                CDebug.Log(this,"unitInfo.targets got modified...",LogType.Error);
            }
        }

        if (firstTargetChange != null && unitInfo.targets.Count > 0 && firstTarget.tileId != unitInfo.targets[0].tileId)
        {
            firstTarget = unitInfo.targets[0];
            firstTargetChange(unitInfo.targets[0]);
        }

        if (targetCountChange != null && previousTargetCount != unitInfo.targets.Count)
        {
            previousTargetCount = unitInfo.targets.Count;

            if (previousTargetCount == 0)
            {
                targetCountChange(null);
            }
            else
            {
                targetCountChange(unitInfo.targets[previousTargetCount - 1]);
            }
        }

    }

    private void TickUpdate()
    {
        GenerateWaypoint(null, true);
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

        CDebug.Log(this,"DISTANCE=" + distance);

        if (distance <= attackDistance && !unitInfo.isEngaged)
        {
            CDebug.Log(this,"NEARBY DISTANCE" + distance);

            AddToIndex(targetUnit.targets, unitInfo, 0);
            targetUnit.currentTarget = unitInfo;

            unitInfo.isEngaged = true;
            targetStandingTile = null;
            ResetCombatPathing();
            targetUnit.waypoints.Add(unitInfo);
        }
        else if (distance <= attackDistance && unitInfo.isEngaged)
        {
            //damage enemy logic here
            CDebug.Log(this, "Unit [" + unitInfo.tileId + "] attacking Unit [" + targetUnit.tileId + "]", LogType.Warning);
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

    private void AddToIndex(List<TileInfo> tileInfos, TileInfo tile, int index)
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
        CDebug.Log(this, "Unit[" + unitInfo.tileId + "] Disengaging!", LogType.Warning);

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
