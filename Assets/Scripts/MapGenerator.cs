/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, January 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class MapGenerator : Pipeline
{
    private static MapGenerator _init;

    public static MapGenerator init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public GameObject placeHolderTile;
    public RectTransform grid;
    public bool useGridSize;
    public int width;
    public int height;
    public float xOffset;
    public float yOffset;
    public float scale;

    protected override void Awake()
    {
        init = this;
        base.Awake();
    }

    public void Initialize(float _xOffset, float _yOffset, float _scale)
    {
        xOffset = _xOffset;
        yOffset = _yOffset;
        scale = _scale;

        foreach (TileInfo tile in TileList.init.generatedTiles.Values)
        {
            Destroy(tile);
        }
        TileList.init.generatedTiles.Clear();

        if (useGridSize)
        {
            RectTransform rectPlaceHolder = placeHolderTile.GetComponent<RectTransform>();
            width = (int)(grid.rect.width / rectPlaceHolder.rect.width);
            height = (int)(grid.rect.height / rectPlaceHolder.rect.height);
        }

        base.Execute();
    }
}
