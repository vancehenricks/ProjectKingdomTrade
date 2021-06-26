/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2019
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class TileInfoGetterArray : MonoBehaviour
{
    public int maxHits;
    //public bool holdList;
    private BoxCollider2D boxCollider2D;
    public List<TileInfo> tileInfos;
    public List<TileInfo> baseTiles;
    private Coroutine scan;

    //private int overflowCount;

    private void Start()
    {
        maxHits = TileInfoRaycaster.init.maxHits;
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void OnDestroy()
    {
      if(scan != null)
      {
          StopCoroutine(scan);
      }
    }

    public void Scan()
    {
      if(scan != null)
      {
          StopCoroutine(scan);
      }
      scan = StartCoroutine(ScanCoroutine());
    }

    private IEnumerator ScanCoroutine()
    {
        Task<List<TileInfo>> task = TileColliderHandler.init.Cast(boxCollider2D.bounds, baseTiles, maxHits);  

        while(!task.IsCompleted)
        {
            yield return null;
        }

        if(!task.IsFaulted && !task.IsCanceled)
        {
            tileInfos.AddRange(task.Result);
        }
    }

    public void Clear()
    {
        //overflowCount = 0;
        //boxCollider2D.enabled = true;
        tileInfos.Clear();
    }
}