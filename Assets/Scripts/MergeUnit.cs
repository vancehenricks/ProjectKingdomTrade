/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, December 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeUnit : PlayerCommand
{
    protected override void Start()
    {
        base.Start();
    }

    public void DoMergeAction()
    {
        DoAction();
        openRightClick.openLeftClick.Ignore();

        for (int i = 1;i < unitInfos.Count;i++)
        {
            unitInfos[i].merge = unitInfos[0];
            unitInfos[i].unitEffect.mergeHandler.GenerateWayPoint();
        }
        EndAction();
    }

    public void DoSplitAction()
    {
        DoAction();
        openRightClick.openLeftClick.Ignore();
        Tools.Split(unitInfos[0]);
        EndAction();
    }
}
