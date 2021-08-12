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
    public Bounds previousBounds;
    public Bounds currentBounds;
    private TileInfo tileInfo;
    private RectTransform rect;

    public System.Action<List<TileInfo>> onEnter, onExit;

    public void Initialize()
    {
        //boxCollider2D = GetComponent<BoxCollider2D>();
        rect = GetComponent<RectTransform>();
        tileInfo = GetComponent<TileInfo>();
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
        TileColliderHandler.init.Remove(tileInfo, currentBounds);  
    }

    public void UpdatePosition()
    {
        previousBounds = currentBounds;
        currentBounds = new Bounds(transform.position,size);
        TileColliderHandler.init.Add(tileInfo, previousBounds, currentBounds);  
    }
    
    public void Relay(bool isEnter = true)
    {
        TileColliderHandler.init.Relay(TileColliderHandler.init.Cast(currentBounds, null, -1/*, tileInfo, isEnter*/), tileInfo, isEnter);        
    }

    //Different thread+
    public void OnCollosion(List<TileInfo> tileInfos, bool isEnter)
    {

        CDebug.Log(nameof(TileCollider), "tileInfos.Count= " + tileInfos.Count, LogType.Warning);

        if(isEnter && onEnter != null)
        {
            onEnter(tileInfos);
        }
        else if(onExit != null)
        {
            onExit(tileInfos);
        }
    }
    //Different thread-
 }