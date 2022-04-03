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
    public TileColliderDictionary colliderDictionary;
    public BaseInfo baseInfo;
    public Bounds bounds;
    public Ray ray;
    public bool useRay;
    public bool filterOut;
    public List<string> filter;
    public int maxHits;

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
    
    private TileColliderDictionary colliderDictionary;

    private void Awake()
    {
        init = this;
    }

    public void Initialize()
    {
        colliderDictionary = new TileColliderDictionary();
    }

    private async Task<List<BaseInfo>> Calculate(TileColliderHandlerValues tileCollider)
    {
        List<BaseInfo> hits = new List<BaseInfo>();
        //tileCollider.colliderDictionary = new TileColliderDictionary(tileCollider.colliderDictionary);
        int hitCount = 0;

        foreach(var colliderValue in tileCollider.colliderDictionary)
        {
            bool hasHits = false;

            if(!tileCollider.useRay)
            {
                hasHits = tileCollider.bounds.Intersects(colliderValue.Key);
            }
            else
            {
                hasHits = colliderValue.Key.IntersectRay(tileCollider.ray);
            }
            
            if(!hasHits) continue;
            
            foreach(BaseInfo tile in colliderValue.Value.Values.ToList<BaseInfo>())
            {                    
                if(hitCount >= tileCollider.maxHits && tileCollider.maxHits != -1) break;

                if(tileCollider.baseInfo != null && tileCollider.baseInfo.tileId == tile.tileId) continue;

                if(tileCollider.filter != null && tileCollider.filter.Count > 0)
                {
                    foreach(string tileType in tileCollider.filter)
                    {
                        if(!tileCollider.filterOut && tile.Contains(tileType) ||
                            tileCollider.filterOut && !tile.Contains(tileType))
                        {
                            hitCount++;
                            hits.Add(tile);
                            break;
                        }
                    }
                }
                else
                {
                    hitCount++;
                    hits.Add(tile);
                }
            }
            
        }

        return await Task.FromResult(hits);
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

        callback(task.Result);         
    }

    private async Task<List<BaseInfo>> Cast(BaseInfo baseInfo, Bounds bounds, Ray ray, bool useRay, 
        List<string> filter, int maxHits, bool filterOut)
    {

        TileColliderHandlerValues colliderHandlerValues = new TileColliderHandlerValues()
        {
            colliderDictionary = colliderDictionary,
            baseInfo = baseInfo,
            bounds = new Bounds(new Vector3(bounds.center.x, bounds.center.y), bounds.size),
            ray = ray,
            useRay = useRay,
            maxHits = maxHits,
            filter = filter,
            filterOut = filterOut,
        };

        Task<List<BaseInfo>> task = Calculate(colliderHandlerValues);
        return await task;
    }

    ///<summary>
    ///Attempts to add or replace current tile in the TileColliderDictionary.<br/>
    ///tile BaseInfo to add.<br/>
    ///previousBounds Previous location of the tile.<br/>
    ///currentBounds The new location of the tile.<br/>
    ///</summary>
    public void Add(BaseInfo tile, Bounds previousBounds, Bounds currentBounds)
    {
        Remove(tile, previousBounds);
        colliderDictionary.TryAdd(currentBounds, new ConcurrentDictionary<long, BaseInfo>());
        colliderDictionary[currentBounds].TryAdd(tile.tileId, tile);        
    }

    ///<summary>
    ///Attempts to remove existing tile in the TileColldierDictionary.<br/>
    ///tile BaseInfo to remove.<br/>
    ///previousBounds Location of the tile.<br/>
    ///</summary>
    public void Remove(BaseInfo tile, Bounds previousBounds)
    {
        BaseInfo removedTile;
        
        if(!colliderDictionary.ContainsKey(previousBounds)) return;
        
        if(!colliderDictionary[previousBounds].TryRemove(tile.tileId, out removedTile) && removedTile != null)
        {
            CDebug.Log(this, "Remove failed for removedTile.tileId=" + removedTile.tileId, LogType.Warning);
        }
    }

}
