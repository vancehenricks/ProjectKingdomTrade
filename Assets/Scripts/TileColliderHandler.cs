/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, June 2021
 */

using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using TileColliderDictionary = System.Collections.Concurrent.ConcurrentDictionary<UnityEngine.Bounds, 
    System.Collections.Concurrent.ConcurrentDictionary<long, BaseInfo>>;

public class TileColliderHandlerValues
{
    public TileColliderDictionary colliderValues;
    public BaseInfo baseInfo;
    public Bounds bounds;
    public Ray ray;
    public bool useRay;
    public bool filterOut;
    public List<string> filter;
    public int maxHits;

    public TileColliderHandlerValues()
    {
        colliderValues = new TileColliderDictionary();
    }

}

///<summary>
///Custom Tile Collusion.<br/>
///</summary>
public class TileColliderHandler : MonoBehaviour
{
    private static TileColliderHandler _init;
    public static TileColliderHandler init
    {
        get { return _init; }
        private set { _init = value; }
    }
    
    private TileColliderHandlerValues colliderHandlerValues;

    private void Awake()
    {
        init = this;
    }

    public void Initialize()
    {
        colliderHandlerValues = new TileColliderHandlerValues();
    }

    private async Task<List<BaseInfo>> Calculate(TileColliderHandlerValues colliderHandlerValues)
    {
        TileColliderDictionary colliderValues = colliderHandlerValues.colliderValues;
        BaseInfo baseInfo = colliderHandlerValues.baseInfo;
        Bounds bounds = colliderHandlerValues.bounds;
        Ray ray = colliderHandlerValues.ray;
        bool useRay = colliderHandlerValues.useRay;
        bool filterOut = colliderHandlerValues.filterOut;
        int maxHits = colliderHandlerValues.maxHits;
        int hitCount = 0;
        List<string> filter = colliderHandlerValues.filter;
        List<BaseInfo> baseInfos = new List<BaseInfo>();

        foreach(var colliderValue in colliderValues)
        {
            bool hasHits = false;

            if(!useRay)
            {
                hasHits = bounds.Intersects(colliderValue.Key);
            }
            else
            {
                hasHits = colliderValue.Key.IntersectRay(ray);
            }
            
            if(!hasHits) continue;
            
            foreach(BaseInfo tile in colliderValue.Value.Values.ToList<BaseInfo>())
            {                    
                if(hitCount >= maxHits && maxHits != -1) break;

                if(baseInfo != null && baseInfo.tileId == tile.tileId) continue;

                if(filter != null && filter.Count > 0)
                {
                    foreach(string tileType in filter)
                    {
                        if(!filterOut && tile.Contains(tileType) ||
                            filterOut && !tile.Contains(tileType))
                        {
                            hitCount++;
                            baseInfos.Add(tile);
                            break;
                        }
                    }
                }
                else
                {
                    hitCount++;
                    baseInfos.Add(tile);
                }
            }
            
        }

        return await Task.FromResult(baseInfos);
    }
    
    ///<summary>
    ///Start a task to check for collusion.<br/>
    ///callback Called upon done with calculation and returns a list of BaseInfo tiles.<br/>
    ///baseInfo Will not include in callback return.<br/>
    ///bounds Use for checking hits in the TileColliderDictionary.<br/>
    ///filter List of tiles to ignore or include in callback depending on filterOut.<br/>
    ///maxHits Number of tiles to check before ending the task.<br/>
    ///</summary>
    public void Cast(System.Action<List<BaseInfo>> callback, BaseInfo baseInfo, Bounds bounds, 
        List<string> filter = null, int maxHits = 1, bool filterOut = false)
    {
        StartCoroutine(GetBaseInfosCoroutine(callback, baseInfo, bounds, new Ray(), false, filter, maxHits, filterOut));
    }

    ///<summary>
    ///Start a task to check for collusion.<br/>
    ///callback Called upon done with calculation and returns a list of BaseInfo tiles.<br/>
    ///ray Use for checking hits in the TileColliderDictionary.<br/>
    ///filter List of tiles to ignore or include in callback depending on filterOut.<br/>
    ///maxHits Number of tiles to check before ending the task.<br/>
    ///</summary>
    public void Cast(System.Action<List<BaseInfo>> callback, Ray ray, 
        List<string> filter = null, int maxHits = 1, bool filterOut = false)
    {
        StartCoroutine(GetBaseInfosCoroutine(callback, null, new Bounds(), ray, true, filter, maxHits, filterOut));
    }    

    private IEnumerator GetBaseInfosCoroutine(System.Action<List<BaseInfo>> callback, 
        BaseInfo baseInfo, Bounds bounds, Ray ray, bool useRay,
        List<string> filter, int maxHits, bool filterOut)
    {
        Task<List<BaseInfo>> task = TileColliderHandler.init.Cast(baseInfo, bounds, ray, useRay, filter, maxHits, filterOut);

        while(!task.IsCompleted)
        {
            yield return null;
        }

        if(task.IsFaulted || task.IsCanceled)
        {
            CDebug.Log(this, "Task Faulted/Canceled", LogType.Error);           
            yield break;
        }

        callback(new List<BaseInfo>(task.Result));         
    }

    private async Task<List<BaseInfo>> Cast(BaseInfo baseInfo, Bounds bounds, Ray ray, bool useRay, 
        List<string> filter, int maxHits, bool filterOut)
    {
        Vector3 center = bounds.center;
        Bounds normalizedBounds = new Bounds(new Vector3(center.x, center.y), bounds.size);

        colliderHandlerValues.baseInfo = baseInfo;
        colliderHandlerValues.bounds = normalizedBounds;
        colliderHandlerValues.ray = ray;
        colliderHandlerValues.useRay = useRay;
        colliderHandlerValues.maxHits = maxHits;
        colliderHandlerValues.filter = filter;
        colliderHandlerValues.filterOut = filterOut;

        Task<List<BaseInfo>> task = Calculate(colliderHandlerValues);
        return await task;
    }

    //Naive approach could cause memory leak

    ///<summary>
    ///Attempts to add or replace current tile in the TileColliderDictionary.<br/>
    ///tile BaseInfo to add.<br/>
    ///previousBounds Previous location of the tile.<br/>
    ///currentBounds The new location of the tile.<br/>
    ///</summary>
    public void Add(BaseInfo tile, Bounds previousBounds, Bounds currentBounds)
    {
        Remove(tile, previousBounds);
        colliderHandlerValues.colliderValues.TryAdd(currentBounds, new ConcurrentDictionary<long, BaseInfo>());
        colliderHandlerValues.colliderValues[currentBounds].TryAdd(tile.tileId, tile);        
    }

    ///<summary>
    ///Attempts to remove existing tile in the TileColldierDictionary.<br/>
    ///tile BaseInfo to remove.<br/>
    ///previousBounds Location of the tile.<br/>
    ///</summary>
    public void Remove(BaseInfo tile, Bounds previousBounds)
    {
        BaseInfo removedTile;
        
        if(!colliderHandlerValues.colliderValues.ContainsKey(previousBounds)) return;
        
        if(!colliderHandlerValues.colliderValues[previousBounds].TryRemove(tile.tileId, out removedTile) && removedTile != null)
        {
            CDebug.Log(this, "Remove failed for removedTile.tileId=" + removedTile.tileId, LogType.Warning);
        }
    }

}
