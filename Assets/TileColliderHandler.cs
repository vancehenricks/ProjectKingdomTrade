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
    System.Collections.Concurrent.ConcurrentDictionary<long, TileInfo>>;

public class TileColliderHandlerValues
{
    public TileColliderDictionary colliderValues;
    public Bounds bounds;
    public Ray ray;
    public bool useRay;
    public TileInfo origin;
    public bool isEnter;
    public List<TileInfo> filter;
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

    private async Task<List<TileInfo>> Calculate(TileColliderHandlerValues colliderHandlerValues)
    {
        TileColliderDictionary colliderValues = colliderHandlerValues.colliderValues;
        Bounds bounds = colliderHandlerValues.bounds;
        Ray ray = colliderHandlerValues.ray;
        bool useRay = colliderHandlerValues.useRay;
        TileInfo origin = colliderHandlerValues.origin;
        bool isEnter = colliderHandlerValues.isEnter;
        int maxHits = colliderHandlerValues.maxHits;
        int hitCount = 0;
        List<TileInfo> filter = colliderHandlerValues.filter;
        List<TileInfo> tileInfos = new List<TileInfo>();

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
                foreach(TileInfo tile in colliderValue.Value.Values.ToList<TileInfo>())
                {
                    if(hitCount > maxHits && maxHits != -1) break;

                    if(filter != null)
                    {
                        foreach(TileInfo fTile in filter)
                        {
                            if(fTile.tileType == tile.tileType)
                            {
                                hitCount++;
                                tileInfos.Add(tile);
                                break;
                            }
                        }
                    }
                    else
                    {
                        hitCount++;
                        tileInfos.Add(tile);
                    }
                }
            }
        }

        if(origin != null)
        {
            List<TileInfo> withOutOrigin = new List<TileInfo>(tileInfos);
            withOutOrigin.Remove(origin);
            
            List<TileInfo> withOrigin = new List<TileInfo>(tileInfos);

            origin.tileEffect.tileCollider.OnCollosion(withOutOrigin, isEnter);

            foreach(TileInfo tile in tileInfos)
            {
                if(origin.tileId == tile.tileId) continue;
                tile.tileEffect.tileCollider.OnCollosion(withOrigin, isEnter);
            }
        }

        return await Task.FromResult(tileInfos);
    }
    
    public Task<List<TileInfo>> Cast(Ray ray, List<TileInfo> filter = null, int maxHits = -1,  bool isEnter = true)
    {
        return Cast(new Bounds(), ray, true, filter, maxHits, null, isEnter);
    }   

    public Task<List<TileInfo>> Cast(Bounds bounds, List<TileInfo> filter = null, int maxHits = -1, TileInfo origin = null, bool isEnter = true)
    {
        return Cast(bounds, new Ray(), false, filter, maxHits, origin, isEnter);
    }

    public async Task<List<TileInfo>> Cast(Bounds bounds, Ray ray, bool useRay, List<TileInfo> filter, int maxHits, TileInfo origin, bool isEnter)
    {
        Vector3 center = bounds.center;
        Bounds normalizedBounds = new Bounds(new Vector3(center.x, center.y), bounds.size);

        //CDebug.Log(this, "colliderValues.Count="+colliderHandlerValues.colliderValues.Count,LogType.Warning);
        colliderHandlerValues.origin = origin;
        colliderHandlerValues.isEnter = isEnter;
        colliderHandlerValues.bounds = normalizedBounds;
        colliderHandlerValues.ray = ray;
        colliderHandlerValues.useRay = useRay;
        colliderHandlerValues.maxHits = maxHits;
        colliderHandlerValues.filter = filter;
        //parallelInstance.Set(colliderHandlerValues);

        Task<List<TileInfo>> task = Calculate(colliderHandlerValues);
        return await task;
    }

    public void Add(TileInfo tile, Bounds previousBounds, Bounds currentBounds)
    {
        TileInfo removedTile;
        if(colliderHandlerValues.colliderValues.ContainsKey(previousBounds))
        {
            colliderHandlerValues.colliderValues[previousBounds].TryRemove(tile.tileId, out removedTile);
        }
        colliderHandlerValues.colliderValues.TryAdd(currentBounds, new ConcurrentDictionary<long, TileInfo>());
        colliderHandlerValues.colliderValues[currentBounds].TryAdd(tile.tileId, tile);        
    }

    public void Remove(TileInfo tile, Bounds previousBounds)
    {
        StartCoroutine(RemoveCoroutine(tile, previousBounds));
    }

    private IEnumerator RemoveCoroutine(TileInfo tile, Bounds previousBounds)
    {
        int timedOut = 30;

        if(!colliderHandlerValues.colliderValues.ContainsKey(previousBounds)) yield break;

        if(colliderHandlerValues.colliderValues[previousBounds].Count <= 1)
        {
            ConcurrentDictionary<long, TileInfo> removedSubGroup;
            while(timedOut > 0 && !colliderHandlerValues.colliderValues.TryRemove(previousBounds, out removedSubGroup))
            {
                timedOut--;
                yield return null;
            }
        }
        else
        {
            TileInfo removedTile;
            while(timedOut > 0 && !colliderHandlerValues.colliderValues[previousBounds].TryRemove(tile.tileId, out removedTile))
            {
                timedOut--;
                yield return null;
            }
        }
    }
}
