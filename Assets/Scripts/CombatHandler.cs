/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, June 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatHandler : MonoBehaviour
{
    public UnitInfo unitInfo;
    public PathFinding pathFinding;
    public delegate void TargetCountChange(TileInfo tile);
    public TargetCountChange targetCountChange;
    public delegate void FirstTargetChange(TileInfo tile);
    public FirstTargetChange firstTargetChange;
    public TileInfoRaycaster tileInfoRaycaster;
    public CombatSession combatSession;

    private CombatSession defaultCombatSession;

    private int previousTargetCount;
    private TileInfo firstTarget;

    private void Start()
    {
        defaultCombatSession = combatSession;
        unitInfo.onEnd += OnEnd;
        Tick.tickUpdate += TickUpdate;
        firstTarget = unitInfo;
    }

    private void OnEnd()
    {
        UnitInfo targetUnit = unitInfo.currentTarget as UnitInfo;

        //DisEngage(targetUnit);
        DisEngage();
        Tick.tickUpdate -= TickUpdate;
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
                    Debug.Log("41");

                    if (target == null)
                    {
                        unitInfo.targets.Remove(target);
                        ResetCombatPathing();
                        break;
                    }

                    //create an instance of CombatSession inside target
                    unitInfo.currentTarget = target;
                    RegisterDelegates();
                    //Issue with this executing the last target since there is no checking here ideal targetted unit will delete itself in unitInfo.targets
                    //This will be catched on target == null
                    //combatSession.ClearCombantants();
                    target.unitEffect.combatHandler.combatSession.Add(target);
                    combatSession = target.unitEffect.combatHandler.combatSession;
                    combatSession.Add(unitInfo);
                    combatSession.Relay();
                }
            }
            catch
            {
                Debug.Log("unitInfo.targets got modified...");
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
            targetCountChange(unitInfo.currentTarget);
        }

    }

    private void FirstWayPointChange(TileInfo tileInfo)
    {
        combatSession.Relay();
    }

    private void WayPointCountChange(TileInfo tileInfo)
    {
        combatSession.Relay();
    }

    private void WayPointReached(TileInfo tileInfo)
    {
        combatSession.Relay();
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

        UnitInfo targetUnit = unitInfo.currentTarget as UnitInfo;
        int distance = GetDistance(targetUnit);
        int attackDistance = unitInfo.attackDistance <= 1 ? 0 : unitInfo.attackDistance;

        Debug.Log("120DISTANCE=" + distance);

        if (distance <= attackDistance && !unitInfo.isEngaged)
        {
            ResetCombatPathing();
            Debug.Log("NEARBY DISTANCE" + distance);
            unitInfo.isEngaged = true;
        }
        else if (distance <= attackDistance && unitInfo.isEngaged)
        {
            Debug.Log("Unit [" + unitInfo.tileId + "] attacking Unit [" + targetUnit.tileId + "]");
        }
        else if (distance > unitInfo.attackDistance && unitInfo.isEngaged)
        {
            Debug.Log("Disengaging!");
            DisEngage();
        }
        else if (distance > unitInfo.attackDistance && !checkOnlyWithinDistance)
        {
            ResetCombatPathing();
            unitInfo.waypoints.Add(waypoint);
        }
    }

    private void RegisterDelegates()
    {
        UnitInfo targetUnit = unitInfo.currentTarget as UnitInfo;
        PathFinding targetPathFinder = targetUnit.unitEffect.pathFinder;

        targetPathFinder.wayPointCountChange += WayPointCountChange;
        targetPathFinder.firstWayPointChange += FirstWayPointChange;
        targetPathFinder.wayPointReached += WayPointReached;
    }

    private void RemoveDelegates()
    {
        UnitInfo targetUnit = unitInfo.currentTarget as UnitInfo;

        if (targetUnit == null) return;

        PathFinding targetPathFinder = targetUnit.unitEffect.pathFinder;

        targetPathFinder.wayPointCountChange -= WayPointCountChange;
        targetPathFinder.firstWayPointChange -= FirstWayPointChange;
        targetPathFinder.wayPointReached -= WayPointReached;
    }

    private int GetDistance(TileInfo targetUnit)
    {
        int distance = 0;

        try
        {
            distance = (int)Vector2.Distance(unitInfo.tileLocation, targetUnit.tileLocation);
        }
        catch
        {
            distance = (int)Vector2.Distance(unitInfo.transform.position, targetUnit.transform.position) / 25;
        }

        return distance;
    }

    private void ResetCombatPathing()
    {
        pathFinding.ResetDestination();
        pathFinding.ResetGeneratedWaypoints();
        unitInfo.waypoints.Clear();
    }

    public void DisEngage()
    {
        CombatHandler unitCombatHandler = unitInfo.unitEffect.combatHandler;

        RemoveDelegates();
        ResetCombatPathing();
        unitInfo.targets.Remove(unitInfo.currentTarget);
        unitCombatHandler.combatSession.Remove(unitInfo);
        unitCombatHandler.combatSession = unitCombatHandler.defaultCombatSession;
        unitInfo.isEngaged = false;
        unitInfo.currentTarget = null;
    }
}
