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
    protected readonly string mergeUnitCommand = "merge-unit log:0 tile-object:0";
    protected readonly string mergeCancelCommand = "merge-unit log:0 tile-object:0 cancel";

    protected override void Start()
    {
        base.Start();
    }

    public override void DoAction()
    {
        base.DoAction();
        OpenLeftClick.init.Ignore();
        ConsoleParser.init.ConsoleEvent(mergeCancelCommand, unitInfos);
        ConsoleParser.init.ConsoleEvent(mergeUnitCommand, unitInfos);

        //UpdateUnitsWayPoints();
        EndAction();
    }
}
