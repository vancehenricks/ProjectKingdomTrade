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

public class ParallelInstance<T>
{
    private Task task;
    private System.Action<System.Action<T,T>,T> action2;
    private System.Action<System.Action<T>,T> action1;    
    private System.Action<T,T> result2;
    private System.Action<T> result1;    
    private T obj;

    public ParallelInstance(System.Action<System.Action<T,T>,T> _action, System.Action<T,T> _result)
    {
        action2 = _action;
        result2 = _result;
    }

    public ParallelInstance(System.Action<System.Action<T>,T> _action, System.Action<T> _result)
    {
        result1 = _result;
        action1 = _action;
    }

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

    public Task Set(T _obj)
    {
        obj = _obj;
        task = new Task(Calculate);
        return task;
    }

    public virtual void Calculate()
    {
        if(action1 != null)
        {
            action1(result1, obj);
        }
        else
        {
            action2(result2, obj);
        }
    }
}
