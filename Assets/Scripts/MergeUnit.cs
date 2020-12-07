﻿/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
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

    public override void DoAction()
    {
        base.DoAction();
        //get unitInfos then assign all units after index 0 with index 0 in merge
        EndAction();
    }

    public override void EndAction()
    {
        base.EndAction();
    }
}
