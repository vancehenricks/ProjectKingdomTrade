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

    public void GetTileInfosFromPos(Vector3 pos, List<TileInfo> _tileInfos)
    {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = pos;

        //This blocks any click within the UIWindows

        List<RaycastResult> results = new List<RaycastResult>();
        graphicsRaycaster.Raycast(pointerEventData, results);

        if (results.Count > 0)
        {
            Debug.Log("results.Count=" + results.Count);
            return;
        }

        _tileInfos.Clear();
        Ray ray = cm.ScreenPointToRay(pos);
        RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray);

        int hitCount = 0;

        foreach (RaycastHit2D hit in hits)
        {
            TileInfo tile = hit.transform.gameObject.GetComponent<TileInfo>();

            if (hitCount <= maxHits && tile != null && tile.tileType != "Edge")
            {
                hitCount++;
                _tileInfos.Add(tile);
            }
        }
    }
}
