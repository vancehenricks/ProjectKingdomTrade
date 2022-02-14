/* Copyright 2022 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2022
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecruitUnitHandler : MonoBehaviour
{
    private TileInfo tileInfo;
    
    private void Start()
    {
        tileInfo = GetComponent<TileInfo>();
        Tick.init.tickUpdate += TickUpdate;
    }

    private void TickUpdate()
    {
        //lock(recruitUnitHandlerInstance)
        //{
            //CDebug.Log(this, "tileId=" + tileInfo.tileId + " TileInfo.recruitInfos.Count=" + tileInfo.recruitInfos.Count, LogType.Warning);

            foreach(var queue in tileInfo.recruitInfos.Values)
            {
                if(queue.Count == 0) continue;
                RecruitInfo recruitInfo = queue.Peek();
                
                if(recruitInfo.timeLeft <= 0)
                {
                    CDebug.Log(this, "spawning unit " + recruitInfo.baseInfo.subType, LogType.Warning);
                    ConsoleParser.init.ConsoleEvent("spawn-unit log:0 player-object:0 sub-type-object:1 tile-object:2", 
                        tileInfo.playerInfo, recruitInfo.baseInfo, tileInfo);
                    queue.Dequeue();
                }

                recruitInfo.timeLeft-=Tick.init.timeDilation;
            }
        //}
    }

    private void OnDestory()
    {
        Tick.init.tickUpdate -= TickUpdate;
    }
}
