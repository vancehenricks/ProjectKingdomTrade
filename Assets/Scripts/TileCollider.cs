/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, June 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
 public class TileCollider : MonoBehaviour
{

    //private BoxCollider2D boxCollider2D;
    public Vector2 size;
    public List<BaseInfo> filter;
    public Bounds previousBounds;
    public Bounds currentBounds;
    private BaseInfo baseInfo;
    private RectTransform rect;

    public System.Action<List<BaseInfo>> onEnter, onStay, onExit;

    public List<BaseInfo> previousBaseInfos;

    public void Initialize()
    {
        //boxCollider2D = GetComponent<BoxCollider2D>();
        rect = GetComponent<RectTransform>();
        baseInfo = GetComponent<BaseInfo>();
        currentBounds = new Bounds(transform.position,size);
        UpdatePosition();
        Relay();
        //boxCollider2D.bounds;         
    }
    
    private void OnDestroy()
    {
        //onCollosion = null;
        onEnter = null;
        onExit = null;
        TileColliderHandler.init.Remove(baseInfo, currentBounds);  
    }

    public void UpdatePosition(bool observer = false)
    {
        previousBounds = currentBounds;
        currentBounds = new Bounds(transform.position,size);

        if(!observer)
        {
            TileColliderHandler.init.Add(baseInfo, previousBounds, currentBounds);  
        }
    }

    public void Listen()
    {
        //still need further improvement
        TileColliderHandler.init.Cast((List<BaseInfo> baseInfos) => {
            //OnCollosion(baseInfos, true);
            
            List<BaseInfo> exit = new List<BaseInfo>();

            if(previousBaseInfos.Count > 0)
            {
                foreach(BaseInfo baseInfo in baseInfos)
                {
                    foreach(BaseInfo prevBaseInfo in previousBaseInfos)
                    {
                        if(baseInfo.tileId == prevBaseInfo.tileId)
                        {
                            exit.Add(baseInfo);
                        }
                    }
                }

                foreach(BaseInfo baseInfo in exit)
                {
                    baseInfos.Remove(baseInfo);
                }
                
                OnCollosion(exit, false);
            }
            
            OnCollosion(baseInfos, true);
            previousBaseInfos = baseInfos;

        }, currentBounds, filter, -1, true);
    }
    
    public void Relay(bool isEnter = true)
    {
        TileColliderHandler.init.Relay(TileColliderHandler.init.Cast(currentBounds, filter, -1, true), baseInfo, isEnter);        
    }

    public void OnCollosion(List<BaseInfo> baseInfos, bool isEnter)
    {

        CDebug.Log(nameof(TileCollider), "baseInfos.tileId=" + baseInfo.tileId + " baseInfos.Count=" + baseInfos.Count, LogType.Warning);

        if(isEnter && onEnter != null)
        {
            onEnter(baseInfos);
        }
        else if(onExit != null)
        {
            onExit(baseInfos);
        }
    }
 }