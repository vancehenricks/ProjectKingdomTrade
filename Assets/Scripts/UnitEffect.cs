/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, August 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitEffect : MonoBehaviour
{

    public UnitInfo unitInfo;
    public TileInfo standingTile;
    public PathFinding pathFinder;
    public CombatHandler combatHandler;
    //public TileInfo previousTile;
    public UnitCycler unitCycler;
    public UnitDirection unitDirection;
    public UnitDisplay unitDisplay;
    public UnitWayPoint unitWayPoint;
    public NonWalkableTiles nonWalkableTiles;

    private bool isRegistered = false;

    private void OnDestroy()
    {
        if (unitCycler != null)
        {
            unitCycler.StopCycle(unitInfo);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TileInfo tempTile = collision.GetComponent<TileInfo>();

        if (tempTile == null)
        {
            return;
        }

        if (tempTile.tileType != unitInfo.tileType)
        {
            standingTile = tempTile;
            //Debug.Log("32 Activate!");
        }

        StopAllCoroutines();
        StartCoroutine(OnTriggerStayDelay());

        unitInfo.tileLocation = tempTile.tileLocation;
        unitInfo.localTemp = tempTile.localTemp;
        unitInfo.travelTime = tempTile.travelTime;
    }

    private IEnumerator OnTriggerStayDelay()
    {
        yield return new WaitForSeconds(1f);

        if (!isRegistered && standingTile != null)
        {
            unitCycler = standingTile.tileEffect.unitCycler;
            unitCycler.StartCycle(unitInfo);
            isRegistered = true;
            //Debug.Log("41 Activate!");
        }

        yield return null;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        /*TileInfo tempTile = collision.GetComponent<TileInfo>();

        if (tempTile == null)
        {
            return;
        }*/

        if (unitInfo.waypoints.Count > 0 || unitInfo.targets.Count > 0)
        {
            //previousTile = tempTile;

            if (isRegistered)
            {
                //TileInfo tempTile = collision.GetComponent<TileInfo>();
                //List<UnitInfo> unitInfos = previousTile.unitInfos;
                //unitInfo.transform.SetAsLastSibling();
                //unitDisplay.instance.transform.SetAsLastSibling();
                unitCycler.StopCycle(unitInfo);
                isRegistered = false;
            }
        }
    }
}
