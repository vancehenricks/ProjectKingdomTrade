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
    public float xSeed;
    public float ySeed;
    public float scale;
    //public int spawnDownChance;
    //public int homogeneousChance;
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
    //private RectTransform rectPlaceHolder;
    //private Vector2 originalPos;

    // Use this for initialization
    /*void Start () {
		GenerateMap();
	}*/

    private List<TileInfo> baseTiles;

    private void Awake()
    {
        _init = this;
        //generatedTile = new Dictionary<Vector2, TileInfo>();
        //borderTile = new List<GameObject>();
    }

    public void Initialize()
    {
        baseTiles = TileConfigHandler.init.baseTiles.Values.ToList<TileInfo>();

        foreach (TileInfo tile in TileList.generatedTiles.Values)
        {
            Destroy(tile);
        }
        TileList.generatedTiles.Clear();
        //borderTile.Clear();

        //originalPos = placeHolderTile.transform.position;

        if (useGridSize)
        {
            RectTransform rectPlaceHolder = placeHolderTile.GetComponent<RectTransform>();
            width = grid.rect.width / rectPlaceHolder.rect.width;
            height = grid.rect.height / rectPlaceHolder.rect.height;
        }
        //placeHolderTile.SetActive(true); no point activating it

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
        Vector3 placeHolder = placeHolderTile.transform.position;
        Vector3 originPos = placeHolder;

        for (float y = 0f; y < height;y++)
        {
            for (float x = 0f; x < width;x++)
            {
                float xCoord = xSeed + x / width * scale;
                float yCoord = ySeed + y / height * scale;
                float spawnHeight = Mathf.PerlinNoise(xCoord, yCoord);

                TileInfo newTile = Instantiate(GetBaseTile(spawnHeight), grid);
                newTile.transform.position = placeHolder;
                newTile.tileLocation = new Vector2(x, y);
                newTile.Initialize();
                newTile.gameObject.SetActive(true);

                placeHolder = new Vector3(placeHolder.x + 25f, placeHolder.y, placeHolder.z);

                //pix[(int)y * noiseTex.width + (int)x] = new Color(sample, sample, sample);
            }

            placeHolder = new Vector3(originPos.x, placeHolder.y - 25, originPos.z);
        }
    }

    private TileInfo GetBaseTile(float spawnHeight)
    {
        //sortedlist is ascending order
        SortedList<float,TileInfo> candidateTiles = new SortedList<float,TileInfo>();

        candidateTiles.Add(baseTiles[0].spawnChance, baseTiles[0]);

        foreach (TileInfo baseTile in baseTiles)
        {
            if (spawnHeight >= baseTile.spawnHeightMin && spawnHeight <= baseTile.spawnHeightMax)
            {
                float spawnChance = baseTile.spawnChance;

                //guarantees no duplication causing exception

                RECHECK:
                if (candidateTiles.ContainsKey(spawnChance))
                {
                    if (Random.Range(0f,1f) > 0.5f)
                    {
                        candidateTiles[baseTile.spawnChance] = baseTile;
                    }

                    spawnChance = baseTile.spawnChance + Random.Range(0f,1f);
                    goto RECHECK;
                }

                candidateTiles.Add(spawnChance, baseTile);
            }
        }

        foreach (TileInfo candidateTile in candidateTiles.Values)
        {
            if (Random.Range(0f, 1f) <= candidateTile.spawnChance)
            {
                return candidateTile;
            }
        }

        return TileConfigHandler.init.baseTiles["Sea"]; //return the largest
    }

}
