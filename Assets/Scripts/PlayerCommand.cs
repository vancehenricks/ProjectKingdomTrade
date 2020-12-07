/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, August 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class PlayerCommand : MonoBehaviour
{
    public TileInfoRaycaster tileInfoRaycaster;
    public OpenRightClick openRightClick;

    protected virtual void Start()
    {
        ExecuteCommands command = Command;
        CommandPipeline.Add(command, 100);
    }

    protected virtual void Command()
    {

    }

    public virtual void DoAction()
    {

    }

    public virtual void EndAction()
    {

    }
}