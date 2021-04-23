/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, April 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandPipeline : Pipeline
{
    private static CommandPipeline _init;
    public static CommandPipeline init
    {
        get { return _init; }
        private set { _init = value; }
    }

    protected override void Awake()
    {
        init = this;
        base.Awake();
    }

    private void Update()
    {
        base.Execute();
    }
}