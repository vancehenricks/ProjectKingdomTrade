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
    private CombatSession _combatSession;
    public CombatSession combatSession 
    {
        private set
        {
            _combatSession = value;
        }
        get
        {
            if (_combatSession == null)
            {
                _combatSession = new CombatSession();
            }

            return _combatSession;
        }
    }

    //private bool[] stop;
    private int previousTargetCount;
    private TileInfo firstTarget;

    private void Start()
    {
        //stop = new bool[6];
        unitInfo.onEnd += OnEnd;
        //Tick.tickUpdate += TickUpdate;
        firstTarget = unitInfo;
    }

    private void OnEnd()
    {
        //Tick.tickUpdate -= TickUpdate;
        targetCountChange = null;
        combatSession = null;
    }

    private void Update()
    {
        if (unitInfo.currentTarget == null)
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
                GenerateWaypoint();
                //Issue with this executing the last target since there is no checking here ideal targetted unit will delete itself in unitInfo.targets
                //combatSession = target.unitEffect.combatHandler.combatSession;
                //combatSession.combatants.Add(unitInfo);
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

    public void GenerateWaypoint()
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
        PathFinding targetPathFinder = targetUnit.unitEffect.pathFinder;
        CombatHandler targetCombatHandler = targetUnit.unitEffect.combatHandler;
        int distance = 0;

        try
        {
            distance = (int)Vector2.Distance(unitInfo.tileLocation, targetUnit.tileLocation);
        }
        catch
        {
            distance = (int)Vector2.Distance(unitInfo.transform.position, targetUnit.transform.position) / 25;
        }

        Debug.Log("120DISTANCE=" + distance);

        if (distance > unitInfo.attackDistance)
        {
            ResetCombatPathing();
            unitInfo.waypoints.Add(targetUnit);
        }
    }

    /*private void CalculateDamage()
    {
        Debug.Log(unitInfo.tileId + " -> " + unitInfo.currentTarget.tileId);
    }*/

    /* private bool HasSameTargetId(UnitInfo targetUnit, UnitInfo unitInfo)
    {
        if (targetUnit.currentTarget == null) return false;

        if (targetUnit.currentTarget.tileId != unitInfo.tileId) return false;

        return true;
    }

    private bool HasSameLastWaypoint(List<TileInfo> waypoint1, List<TileInfo> waypoint2)
    {

        if (waypoint1.Count == 0 || waypoint2.Count == 0) return false;

        int maxIndexA = waypoint1.Count - 1,
            maxIndexB = waypoint2.Count - 1;

        if (waypoint1[maxIndexA] != waypoint2[maxIndexB]) return false;

        return true;
    }

    public void SetStop(bool val)
    {
        for (int i = 0; i < stop.Length; i++)
        {
            stop[i] = val;
        }
    }

    public bool IsAllStop(bool value, params int[] exempt)
    {
        for (int i = 0; i < stop.Length; i++)
        {
            bool skip = false;

            foreach (int index in exempt)
            {
                if (index == i)
                {
                    if (stop[i] != value)
                    {
                        skip = true;
                        break;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            if (!skip && stop[i] != value) return false;
        }

        return true;
    }*/

    public void ResetCombatPathing()
    {
        pathFinding.ResetDestination();
        pathFinding.ResetGeneratedWaypoints();
        unitInfo.waypoints.Clear();
    }

    //NewWaypoint
    //target to null


}


/*
     
        //Issue with some situation when both attack and does not land proper tile
        //Issue with three units attacking 1 unit then decide to retreat

        if (unitInfo.currentTarget == null)
        {
            if (unitInfo.targets.Count > 0)
            {
                ResetCombatPathing();
            }

            SetStop(false);
            return;
        }

        UnitInfo targetUnit = unitInfo.currentTarget as UnitInfo;
        PathFinding targetPathFinder = targetUnit.unitEffect.pathFinder;
        CombatHandler targetCombatHandler = targetUnit.unitEffect.combatHandler;
        int distance = 0;

        try
        {
            distance = (int)Vector2.Distance(unitInfo.tileLocation, targetUnit.tileLocation);
        }
        catch
        {
            distance = (int)Vector2.Distance(unitInfo.transform.position, targetUnit.transform.position) / 25;
        }

        Debug.Log("96DISTANCE=" + distance);

        //defender
        if (IsAllStop(true) && distance <= unitInfo.attackDistance)
        {
            stop[4] = false;
        }
        else if (IsAllStop(true, 4) && distance > unitInfo.attackDistance ||
stop[3] && !stop[4] && distance > unitInfo.attackDistance)
        {
            Debug.Log(stop[4]);
            Debug.Log("TILEID"+unitInfo.tileId);

            unitInfo.targets.Remove(unitInfo.currentTarget);

            stop[5] = false; //issue with re-selecting enemy [fix]

            if (unitInfo.targets.Count == 0)
            {
                unitInfo.currentTarget = null;
                return;
            }
        }

        if (distance <= unitInfo.attackDistance)
        {
            CalculateDamage();
        }

        //attacker
        if (!stop[3] && distance <= unitInfo.attackDistance)
        {
            SetStop(false);
stop[3] = true;

            if (targetUnit != null && targetUnit.attackDistance == unitInfo.attackDistance)
            {
                targetCombatHandler.ResetCombatPathing();
            }
            else
            {
                targetCombatHandler.ResetCombatPathing();
                targetUnit.waypoints.Add(unitInfo);
            }

            ResetCombatPathing();

            //Debug.Log(unitInfo.tileId + " -> " + unitInfo.currentTarget.tileId);

            if (unitInfo.currentTarget != null && distance <= unitInfo.currentTarget.attackDistance && !unitInfo.currentTarget.targets.Contains(unitInfo))
            {
                unitInfo.currentTarget.currentTarget = unitInfo;
                unitInfo.currentTarget.targets.Insert(0, unitInfo);
            }

            return;
        }
        else if (unitInfo.currentTarget.currentTarget != null && !stop[0] && unitInfo.currentTarget.currentTarget.tileId == unitInfo.tileId)
        {
            int count = pathFinding.generatedWayPoints.Count;
int index = (count / 2);

            if (targetUnit == null || count == 0) return;

            Debug.Log("pathFinding.gwPointsIndex= " + pathFinding.gwPointsIndex + " Count=" + count + " index=" + index);

            if (pathFinding.gwPointsIndex > index)
            {
                Debug.Log("RESETTING");
                ResetCombatPathing();
unitInfo.waypoints.Add(targetUnit);
                return;
            }

            stop[0] = true;
            targetCombatHandler.ResetCombatPathing();
            targetCombatHandler.SetStop(true);
            targetUnit.waypoints.Add(pathFinding.generatedWayPoints[index]);

        }
        else if (!stop[0] && !stop[1] && distance > unitInfo.attackDistance)
        {
            Debug.Log("stop1 re-calculating");
            stop[1] = true;
            ResetCombatPathing();
unitInfo.waypoints.Add(unitInfo.currentTarget);
        }
        else if (targetPathFinder != null && targetUnit != null && !stop[0] && targetPathFinder.generatedWayPoints.Count > 0 &&
                    (
                        (!stop[2] && stop[1]) ||
                        (stop[2] && stop[1] && !HasSameTargetId(targetUnit, unitInfo) && 
                        !HasSameLastWaypoint(pathFinding.generatedWayPoints, targetPathFinder.generatedWayPoints))
                    )
                )
        {
            Debug.Log("stop2 re-calculating");

            stop[2] = true;
            ResetCombatPathing();
unitInfo.waypoints.Add(targetPathFinder.generatedWayPoints[targetPathFinder.generatedWayPoints.Count - 1]);
        }



     */
