
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///One way of implementing a ParallalInstance.<br/>
///T Object/struct to be used for calculation.<br/>
///L Object/struct to be converted to T.<br/>
///</summary>
public interface IParallelContract<T,L>
{
    List<T> Convert(List<L> list);
    void Calculate(System.Action<List<T>> result, List<T> list); 
    IEnumerator Sync();
}