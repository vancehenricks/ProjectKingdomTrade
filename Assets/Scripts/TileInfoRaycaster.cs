/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, June 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Threading.Tasks;

public class TileInfoRaycaster : MonoBehaviour
{
    private static TileInfoRaycaster _init;
    public static TileInfoRaycaster init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public Camera cm;
    public int maxHits;
    public List<TileInfo> tileInfos;

    public GraphicRaycaster graphicsRaycaster;
    public EventSystem eventSystem;

    private PointerEventData pointerEventData;

    private void Awake()
    {
        init = this;
        //tileInfos = new List<TileInfo>();
    }

    public void GetTileInfosFromPos(Vector3 pos)
    {
        GetTileInfosFromPos(pos, tileInfos);
    }

    public TileInfo GetTileInfoFromPos(Vector3 pos)
    {
        List<TileInfo> tileInfos = new List<TileInfo>();
        GetTileInfosFromPos(pos, tileInfos);

        if (tileInfos.Count == 0) return null;

        return tileInfos[0];
    }

    public void GetTileInfosFromPos(Vector3 pos, List<TileInfo> _tileInfos, List<TileInfo> filter = null)
    {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = pos;

        //This blocks any click within the UIWindows

        List<RaycastResult> results = new List<RaycastResult>();
        graphicsRaycaster.Raycast(pointerEventData, results);

        if (results.Count > 0)
        {
            CDebug.Log(this, "UI hit=" + results[results.Count-1].gameObject.name);
            return;
        }

        _tileInfos.Clear();
        StartCoroutine(GetTilesInfoFromPosCoroutine(pos , _tileInfos, filter));
        CDebug.Log(this, "_tileInfos.Count=" + _tileInfos.Count, LogType.Warning);  
    }

    private IEnumerator GetTilesInfoFromPosCoroutine(Vector3 pos, List<TileInfo> _tileInfos, List<TileInfo> filter = null)
    {
         Ray ray = cm.ScreenPointToRay(pos);
        //bounds.extents = Vector3.one;
        Task<List<TileInfo>> task = TileColliderHandler.init.Cast(ray, filter, maxHits);

        while(!task.IsCompleted)
        {
            yield return null;
        }

        if(!task.IsFaulted && !task.IsCanceled)
        {
            //Make sure to add it instead of assigning reference Result directly, this will cause issue of not retrieving tiles in unity
            _tileInfos.AddRange(task.Result);
        }

        CDebug.Log(this, "_tileInfos.Count=" + _tileInfos.Count, LogType.Warning);   
    }
}
