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

public class MapGenerator : MonoBehaviour
{
    private static MapGenerator _init;

    public static MapGenerator init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public GameObject placeHolderTile;
    public RectTransform grid;
    public float distanceOfEachTile;
    public bool useGridSize;
    public float width;
    public float height;
    public int spawnDownChance;
    public int homogeneousChance;
    //public List<GameObject> generatedTile;
    //public List<GameObject> borderTile;
    //public Dictionary<Vector2, TileInfo> generatedTile;

    public delegate void OnGenerateTile(ref TileInfo tile, GameObject placeHolderTile, Vector2 location, int x, int y);
    public OnGenerateTile onGenerateTile;

    public delegate void OnNewLine(int x, int y);
    public OnNewLine onNewLine;

    public delegate void OnDoneGenerate();
    public OnDoneGenerate onDoneGenerate;

    public delegate void OnInitialize();
    public OnInitialize onInitialize;

    //private List<int> _borderIndex;
    private RectTransform rectPlaceHolder;
    private Vector2 originalPos;

    // Use this for initialization
    /*void Start () {
		GenerateMap();
	}*/

    private void Awake()
    {
        _init = this;
        //generatedTile = new Dictionary<Vector2, TileInfo>();
        //borderTile = new List<GameObject>();
    }

    public void Initialize()
    {

        foreach (TileInfo tile in TileList.generatedTiles.Values)
        {
            Destroy(tile);
        }
        TileList.generatedTiles.Clear();
        //borderTile.Clear();

        originalPos = placeHolderTile.transform.position;

        if (useGridSize)
        {
            rectPlaceHolder = placeHolderTile.GetComponent<RectTransform>();
            width = grid.rect.width / rectPlaceHolder.rect.width;
            height = grid.rect.height / rectPlaceHolder.rect.height;
        }
        placeHolderTile.SetActive(true);

        if (onInitialize != null)
        {
            onInitialize();
        }
        GenerateMapLogic();
        onInitialize = null;
        onGenerateTile = null;
        onNewLine = null;
        onDoneGenerate = null;
    }

    private int GetBaseTileIndex(TileInfo tileInfo)
    {
        List<TileInfo> baseTiles = TileConfigHandler.init.baseTiles.Values.ToList<TileInfo>();

        for (int i = 0; i < baseTiles.Count; i++)
        {
            TileInfo bTile = baseTiles[i];

            if (bTile.tileType == tileInfo.tileType)
            {
                return i;
            }
        }

        return 0;
    }

    private void GenerateMapLogic()
    {
        List<TileInfo> baseTiles = TileConfigHandler.init.baseTiles.Values.ToList<TileInfo>();
        


    }

}
