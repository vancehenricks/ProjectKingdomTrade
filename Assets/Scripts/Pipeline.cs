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
    public static SortedList<float, ExecuteCommands> commandList;

    private void Awake()
    {
        commandList = new SortedList<float, ExecuteCommands>();
        OnAwake();
    }

    public virtual void OnAwake()
    {
        //do nothing
    }

    public void Execute()
    {
        foreach (var command in commandList.Values)
        {
            //Debug.Log(command.Method.ToString());
            command();
        }
    }

    public static void Add(ExecuteCommands command, float priority)
    {
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
