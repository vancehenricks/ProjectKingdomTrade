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
    private System.Action<System.Action<T,T>,T> action;
    private System.Action<T,T> result;
    private T obj;

    public ParallelInstance(System.Action<System.Action<T,T>,T> _action, System.Action<T,T> _result)
    {
        action = _action;
        result = _result;
    }

    public virtual void Set(T _obj)
    {
        obj = _obj;
    }

    public virtual void Calculate()
    {
        action(result, obj);
    }
}
