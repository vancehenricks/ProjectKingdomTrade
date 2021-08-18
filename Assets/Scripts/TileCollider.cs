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

    public System.Action<List<BaseInfo>> onEnter, onExit;

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

    public void UpdatePosition()
    {
        previousBounds = currentBounds;
        currentBounds = new Bounds(transform.position,size);
        TileColliderHandler.init.Add(baseInfo, previousBounds, currentBounds);  
    }
    
    public void Relay(bool isEnter = true)
    {
        TileColliderHandler.init.Relay(TileColliderHandler.init.Cast(currentBounds, filter, -1, true), baseInfo, isEnter);        
    }

    //Different thread+
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
    //Different thread-
 }