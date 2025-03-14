﻿/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudEffectGround : MonoBehaviour
{
    public CloudAction cloudAction;
    private int tickCount = 0;

    private void Start()
    {
        cloudAction.tileCollider.onEnter += OnEnter;
        cloudAction.tileCollider.onExit += OnExit;
        cloudAction.tileCollider.Initialize();
        Tick.init.tickUpdate += TickUpdate;
    }

    private void OnDestroy()
    {
        cloudAction.tileCollider.onEnter -= OnEnter;
        cloudAction.tileCollider.onExit -= OnExit;
        Tick.init.tickUpdate -= TickUpdate;
    }

    private void TickUpdate()
    {
        if(cloudAction.tickCountMax < 0) return;

        if(tickCount++ > cloudAction.tickCountMax)
        {
            tickCount = 0;
            cloudAction.tileCollider.UpdatePosition();
            cloudAction.tileCollider.Listen();
        }
    }

    private void OnEnter(List<BaseInfo> baseInfos)
    {

        foreach(BaseInfo baseInfo in baseInfos)
        {
            TileInfo tileInfo = baseInfo as TileInfo; 

            if (tileInfo != null)
            {
                tileInfo.tileEffect.UpdateTileEffect(cloudAction, false);
            }

            CloudAction cloud = baseInfo as CloudAction;

            if (cloud != null) 
            {
                if (cloudAction.subType == "Cloud" && cloud.subType == "Cloud")
                {
                    CloudCycle.init.GenerateTornado(cloudAction, cloud);
                }
                else if (cloudAction.subType == "Cloud")
                {
                    cloudAction.markedForDestroy = true;
                }
                else if (cloudAction.subType == "Tornado" && cloud.subType == "Cloud")
                {
                    cloudAction.liveTimeCounter -= cloud.collidePoints;
                }
                else if (cloudAction.subType == "Tornado" && cloud.subType == "Tornado")
                {
                    CloudCycle.init.GenerateTornado(cloudAction, cloud);
                }
            }
        }
    }

    private void OnExit(List<BaseInfo> baseInfos)
    {
        foreach(BaseInfo baseInfo in baseInfos)
        {
            //CDebug.Log(this, "baseInfo.tileType=" + baseInfo.tileType, LogType.Warning);

            TileInfo tileInfo = baseInfo as TileInfo; 

            if (tileInfo != null)
            {
                tileInfo.tileEffect.UpdateTileEffect(cloudAction, true);
            }
        }
    }

}
