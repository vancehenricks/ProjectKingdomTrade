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
    public Bounds bounds;
    public Ray ray;
    public bool useRay;
    public bool filterOut;
    //public BaseInfo origin;
    //public bool isEnter;
    public List<BaseInfo> filter;
    public int maxHits;

    public TileColliderHandlerValues()
    {
        colliderValues = new TileColliderDictionary();
    }

}

public class TileColliderHandler : MonoBehaviour
{
    private static TileColliderHandler _init;
    public static TileColliderHandler init
    {
        get { return _init; }
        private set { _init = value; }
    }
    
    //private ParallelInstance<TileColliderHandlerValues> parallelInstance;
    
    private TileColliderHandlerValues colliderHandlerValues;

    private void Awake()
    {
        init = this;
    }

    public void Initialize()
    {
        //Tick.init.tickUpdate += TickUpdate;
        colliderHandlerValues = new TileColliderHandlerValues();
        //parallelInstance = new ParallelInstance<TileColliderHandlerValues>(Calculate);
    }

    private async Task<List<BaseInfo>> Calculate(TileColliderHandlerValues colliderHandlerValues)
    {
        TileColliderDictionary colliderValues = colliderHandlerValues.colliderValues;
        Bounds bounds = colliderHandlerValues.bounds;
        Ray ray = colliderHandlerValues.ray;
        bool useRay = colliderHandlerValues.useRay;
        bool filterOut = colliderHandlerValues.filterOut;
        //BaseInfo origin = colliderHandlerValues.origin;
        //bool isEnter = colliderHandlerValues.isEnter;
        int maxHits = colliderHandlerValues.maxHits;
        int hitCount = 0;
        List<BaseInfo> filter = colliderHandlerValues.filter;
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
            
            if(hasHits)
            {
                foreach(BaseInfo tile in colliderValue.Value.Values.ToList<BaseInfo>())
                {
                    if(hitCount > maxHits && maxHits != -1) break;

                    if(filter != null && filter.Count > 0)
                    {
                        foreach(BaseInfo fTile in filter)
                        {
                            if(!filterOut)
                            {
                                if(fTile.tileType == tile.tileType)
                                {
                                    hitCount++;
                                    baseInfos.Add(tile);
                                    break;
                                }
                            }
                            else if(fTile.tileType != tile.tileType)
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
        }

        /*if(origin != null)
        {
            List<BaseInfo> withOutOrigin = new List<BaseInfo>(baseInfos);
            withOutOrigin.Remove(origin);
            
            List<BaseInfo> withOrigin = new List<BaseInfo>(baseInfos);

            origin.tileEffect.tileCollider.OnCollosion(withOutOrigin, isEnter);

            foreach(BaseInfo tile in baseInfos)
            {
                if(origin.tileId == tile.tileId) continue;
                tile.tileEffect.tileCollider.OnCollosion(withOrigin, isEnter);
            }
        }*/

        return await Task.FromResult(baseInfos);
    }
    
    public Task<List<BaseInfo>> Cast(Ray ray, List<BaseInfo> filter = null, int maxHits = -1, bool filterOut = false  /*, bool isEnter = true*/)
    {
        return Cast(new Bounds(), ray, true, filter, maxHits, filterOut);
    }   

    public Task<List<BaseInfo>> Cast(Bounds bounds, List<BaseInfo> filter = null, int maxHits = -1, bool filterOut = false /*, BaseInfo origin = null, bool isEnter = true*/)
    {
        return Cast(bounds, new Ray(), false, filter, maxHits, filterOut);
    }

    public async Task<List<BaseInfo>> Cast(Bounds bounds, Ray ray, bool useRay, List<BaseInfo> filter, int maxHits, bool filterOut /*, BaseInfo origin, bool isEnter*/)
    {
        Vector3 center = bounds.center;
        Bounds normalizedBounds = new Bounds(new Vector3(center.x, center.y), bounds.size);

        //CDebug.Log(this, "colliderValues.Count="+colliderHandlerValues.colliderValues.Count,LogType.Warning);
        //colliderHandlerValues.origin = origin;
        //colliderHandlerValues.isEnter = isEnter;
        colliderHandlerValues.bounds = normalizedBounds;
        colliderHandlerValues.ray = ray;
        colliderHandlerValues.useRay = useRay;
        colliderHandlerValues.maxHits = maxHits;
        colliderHandlerValues.filter = filter;
        colliderHandlerValues.filterOut = filterOut;
        //parallelInstance.Set(colliderHandlerValues);

        Task<List<BaseInfo>> task = Calculate(colliderHandlerValues);
        return await task;
    }

    public void Relay(Task<List<BaseInfo>> task, BaseInfo origin, bool isEnter)
    {
        StartCoroutine(RelayCoroutine(task, origin, isEnter));
    }

    private IEnumerator RelayCoroutine(Task<List<BaseInfo>> task, BaseInfo origin, bool isEnter)
    {
        while(!task.IsCompleted)
        {
            yield return null;
        }

        if(task.IsFaulted || task.IsCanceled) yield break;

        List<BaseInfo> baseInfos = task.Result;
        origin.tileCollider.OnCollosion(baseInfos, isEnter);
    }

    //Naive approach could cause memory leak
    public void Add(BaseInfo tile, Bounds previousBounds, Bounds currentBounds)
    {
        Remove(tile, previousBounds);
        colliderHandlerValues.colliderValues.TryAdd(currentBounds, new ConcurrentDictionary<long, BaseInfo>());
        colliderHandlerValues.colliderValues[currentBounds].TryAdd(tile.tileId, tile);        
    }

    public void Remove(BaseInfo tile, Bounds previousBounds)
    {
        BaseInfo removedTile;
        if(colliderHandlerValues.colliderValues.ContainsKey(previousBounds))
        {
            if(!colliderHandlerValues.colliderValues[previousBounds].TryRemove(tile.tileId, out removedTile) && removedTile != null)
            {
                CDebug.Log(this, "Remove failed for removedTile.tileId=" + removedTile.tileId, LogType.Warning);
            }
        }
    }

}
