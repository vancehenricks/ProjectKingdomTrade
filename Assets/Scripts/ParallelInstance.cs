/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, May 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

/// <summary>
/// Helper class in creating task.
/// </summary>
public class ParallelInstance<T>
{
    private Task task;
    private System.Action<System.Action<T>,T> action1;    
    private System.Action<T> result1;    
    private T obj;

    ///<summary>
    ///_action Callback containing business logic for the task.
    ///_result Callback containing result of _action.
    ///Return an instance of a task.
    ///Note: Make sure to call _result callback inside _action.
    ///</summary>
    public ParallelInstance(System.Action<System.Action<T>,T> _action, System.Action<T> _result)
    {
        result1 = _result;
        action1 = _action;
    }

    ///<summary>
    ///Sets _obj then call Start().
    ///_obj Contains read-only value to generate result.
    ///Returns an instance of a task.
    ///</summary>
    public virtual Task Start(T _obj)
    {
        obj = _obj;
        task = new Task(Calculate);
        task.Start();

        return task;
    }

    public virtual Task Start()
    {
        task.Start();
        return task;
    }

    ///<summary>
    ///Set _obj data then return task which you can call Start().
    ///_obj Contains read-only value to generate result.
    ///Returns an instance of a task.
    ///</summary>
    public Task Set(T _obj)
    {
        obj = _obj;
        task = new Task(Calculate);
        return task;
    }

    protected virtual void Calculate()
    {
        action1(result1, obj);
    }
}
