
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///One way of implementing a ParallalInstance.
///</summary>
public interface IParallelContract<T,L>
{
    List<T> Convert(List<L> list);
    void Calculate(System.Action<List<T>> result, List<T> list); 
    IEnumerator Sync();
}