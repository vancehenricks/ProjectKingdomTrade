/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, November 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectTiles : MonoBehaviour
{
    [SerializeField] private GameObject baseSelect;
    public OpenRightClick openRightClick;
    [SerializeField] private List<GameObject> selects;

    private void Start()
    {
        selects = new List<GameObject>();
        MultiSelect.onSelectedChange += OnSelectedChange;
    }

    private void OnSelectedChange(List<TileInfo> tileInfos)
    {
        if (openRightClick.include.Count > 0) return;

        if (tileInfos.Count != 0)
        {
            Clear();
        }

        foreach (TileInfo tile in tileInfos)
        {
            Select(tile);
        }
    }

    public void Select(TileInfo tile)
    {
        if (tile.tileType == "Edge") return;
    }

    private void Clear()
    {
        foreach (GameObject select in selects)
        {
            Destroy(select);
        }
    }
}
