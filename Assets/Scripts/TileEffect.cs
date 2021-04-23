/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TileEffect : MonoBehaviour
{

    public TileInfo tileInfo;
    public Image image;
    public Image border;
    public Sprite borderClaimed;
    public Sprite borderUnClaimed;
    public Sprite borderConflict;
    public Sprite freezingTile;
    public Sprite autumnTile;
    public Sprite summerTile;
    public Sprite springTile;
    public float freezingTemp;
    public float autumnTemp;
    public float summerTemp;
    public UnitCycler unitCycler;
    public UnitWayPoint UnitWayPoint;
    //public int maxDaysBeforeRevert;

    //private int daysResultWinter;
    //private int daysResult;

    //private int startedDay;

    public void UpdateTileEffect()
    {

        if (tileInfo.tileType == "Edge") return;

        //daysResult = Tick.init.realDays-startedDay;
        if (tileInfo.localTemp <= summerTemp && ClimateControl.init.isSpring)
        {
            image.sprite = springTile;
            //image.sprite = Temperature.temperature;
        }

        if (tileInfo.localTemp >= summerTemp && ClimateControl.init.isSummer)
        {
            image.sprite = summerTile;
        }

        if (tileInfo.localTemp <= autumnTemp && ClimateControl.init.isAutumn)
        {
            image.sprite = autumnTile;
        }

        if (tileInfo.localTemp <= freezingTemp && ClimateControl.init.isWinter)
        {
            image.sprite = freezingTile;
        }
    }

    public void UpdateTerritoryColor()
    {
        if (tileInfo.tileType == "Edge") return;

        border.color = tileInfo.playerInfo.color;
        if (tileInfo.claimants.Count == 1)
        {
            border.sprite = borderClaimed;
        }
        else if (tileInfo.claimants.Count > 1)
        {
            border.sprite = borderConflict;
        }
        else
        {
            border.sprite = borderUnClaimed;
        }

        if (tileInfo.tileType == "Town") return;

        border.color /= tileInfo.claimants.Count;
        border.color = new Color(border.color.r, border.color.g, border.color.b);
    }
}
