/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, Febuary 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ExecuteCommands();

public class Pipeline : MonoBehaviour
{
    private static Pipeline _init;
    public static Pipeline init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public SortedList<float, ExecuteCommands> commandList;

    protected virtual void Awake()
    {
        init = this;
        commandList = new SortedList<float, ExecuteCommands>();
    }

    protected void OnDestroy()
    {
        commandList = null;
    }

    public void Execute()
    {
        //did not add try catch on this as it can recover right after error
        foreach (var command in commandList.Values)
        {
            //Debug.Log(command.Method.ToString());
            command();
        }
    }

    public void Add(ExecuteCommands command, float priority)
    {
        if (commandList == null)
        {
            commandList = new SortedList<float, ExecuteCommands>();
        }

        while (true)
        {
            if (commandList.ContainsKey(priority))
            {
                priority += 0.001f;
            }
            else
            {
                break;
            }
        }

        commandList.Add(priority, command);
    }
}
