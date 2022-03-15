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

    public List<string> filterOut;

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
        GetTileInfosFromPos(pos, tileInfos, 1);

        if (tileInfos.Count == 0) return null;

        return tileInfos[0];
    }

    public void GetTileInfosFromPos(Vector3 pos, List<TileInfo> _tileInfos, int hits = -1)
    {
        if(hits == -1)
        {
            hits = maxHits;
        }

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

        Ray ray = cm.ScreenPointToRay(pos);

        TileColliderHandler.init.Cast((List<BaseInfo> baseInfos) => {
            _tileInfos.AddRange(Tools.ConvertBaseToTileInfo(baseInfos));
        }, ray, filterOut, hits, true);

        CDebug.Log(this, "_tileInfos.Count=" + _tileInfos.Count);  
    }
}
