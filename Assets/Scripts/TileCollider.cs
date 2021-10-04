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
    public List<string> filterOut;
    public Bounds previousBounds;
    public Bounds currentBounds;
    public BaseInfo baseInfo;
    private RectTransform rect;

    public System.Action<List<BaseInfo>> onEnter, onExit;

    public List<BaseInfo> previousBaseInfos;

    public void Initialize(bool observer = false)
    {
        //boxCollider2D = GetComponent<BoxCollider2D>();
        rect = GetComponent<RectTransform>();
        baseInfo = GetComponent<BaseInfo>();
        currentBounds = new Bounds(new Vector3(transform.position.x,transform.position.y,0f),size);

        if(!observer)
        {
            UpdatePosition();
            Relay();
        }
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
        //CDebug.Log(this, "baseInfo.tileId=" + baseInfo.tileId + " transform.position=" + transform.position, LogType.Warning);
        currentBounds = new Bounds(new Vector3(transform.position.x,transform.position.y,0f),size);

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
            baseInfos.Remove(baseInfo);
            
            List<BaseInfo> exit = new List<BaseInfo>();

            if(previousBaseInfos.Count > 0)
            {
                foreach(BaseInfo bInfo in baseInfos)
                {
                    foreach(BaseInfo prevBaseInfo in previousBaseInfos)
                    {
                        if(bInfo.tileId == prevBaseInfo.tileId)
                        {
                            exit.Add(bInfo);
                        }
                    }
                }

                foreach(BaseInfo bInfoExit in exit)
                {
                    baseInfos.Remove(bInfoExit);
                }
                
                OnCollosion(exit, false);
            }
            
            OnCollosion(baseInfos, true);
            previousBaseInfos = baseInfos;

        }, baseInfo, currentBounds, filterOut, -1, true);
    }
    
    public void Relay(bool isEnter = true)
    {
        TileColliderHandler.init.Cast((List<BaseInfo> baseInfos) => {
        baseInfos.Remove(baseInfo);
        
        OnCollosion(baseInfos, isEnter);

        }, baseInfo, currentBounds, filterOut, -1, true);      
    }

    public void OnCollosion(List<BaseInfo> baseInfos, bool isEnter)
    {

        //CDebug.Log(nameof(TileCollider), "baseInfos.tileId=" + baseInfo.tileId + " baseInfos.Count=" + baseInfos.Count, LogType.Warning);

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