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

public class MapGenerator : MonoBehaviour
{
    private static MapGenerator _init;

    public static MapGenerator init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public List<GameObject> baseTile;
    public GameObject placeHolderTile;
    public GameObject grid;
    public float distanceOfEachTile;
    public bool useGridSize;
    public float width;
    public float height;
    public int spawnDownChance;
    public int homogeneousChance;
    //public List<GameObject> generatedTile;
    //public List<GameObject> borderTile;
    public Dictionary<Vector2, GameObject> generatedTile;

    public delegate void OnGenerateTile(ref GameObject tile, GameObject placeHolderTile, Vector2 location, int x, int y);
    public OnGenerateTile onGenerateTile;

    public delegate void OnNewLine(int x, int y);
    public OnNewLine onNewLine;

    public delegate void OnDoneGenerate();
    public OnDoneGenerate onDoneGenerate;

    public delegate void OnInitialize();
    public OnInitialize onInitialize;

    private List<int> _borderIndex;
    private RectTransform rectGrid;
    private RectTransform rectPlaceHolder;
    private Vector2 originalPos;

    // Use this for initialization
    /*void Start () {
		GenerateMap();
	}*/

    private void Awake()
    {
        _init = this;
        generatedTile = new Dictionary<Vector2, GameObject>();
        //borderTile = new List<GameObject>();
    }

    public void Initialize()
    {

        foreach (GameObject tile in generatedTile.Values)
        {
            Destroy(tile);
        }
        generatedTile.Clear();
        //borderTile.Clear();

        originalPos = placeHolderTile.transform.position;

        if (useGridSize)
        {
            rectGrid = grid.GetComponent<RectTransform>();
            rectPlaceHolder = placeHolderTile.GetComponent<RectTransform>();
            width = rectGrid.rect.width / rectPlaceHolder.rect.width;
            height = rectGrid.rect.height / rectPlaceHolder.rect.height;
        }
        placeHolderTile.SetActive(true);
        onInitialize();
        GenerateMapLogic();
        onInitialize = null;
        onGenerateTile = null;
        onNewLine = null;
        onDoneGenerate = null;
    }

    /*
		up		    current index - width	
		down	    current index + width	
		left	    current index - 1	
		right	    current index + 1
        up-left     current index - width)-1
        up-right    current index - width)+1
        down-left   current index + width)-1
        down-right  current index - width)+1
	*/


    public GameObject GetObjectFrom(Vector2 tileLocation, Direction direction)
    {
        Vector2 key = Vector2.zero;
        GameObject obj = null;

        switch (direction)
        {
            case Direction.Up:
                key = new Vector2(tileLocation.x - 1, tileLocation.y);
                break;
            case Direction.UpRight:
                key = new Vector2(tileLocation.x - 1, tileLocation.y + 1);
                break;
            case Direction.Right:
                key = new Vector2(tileLocation.x, tileLocation.y + 1);
                break;
            case Direction.DownRight:
                key = new Vector2(tileLocation.x + 1, tileLocation.y + 1);
                break;
            case Direction.Down:
                key = new Vector2(tileLocation.x + 1, tileLocation.y);
                break;
            case Direction.DownLeft:
                key = new Vector2(tileLocation.x + 1, tileLocation.y - 1);
                break;
            case Direction.Left:
                key = new Vector2(tileLocation.x, tileLocation.y - 1);
                break;
            case Direction.UpLeft:
                key = new Vector2(tileLocation.x - 1, tileLocation.y - 1);
                break;
        }

        generatedTile.TryGetValue(key, out obj);

        return obj;
    }

    public int GetBaseTileIndex(GameObject obj)
    {
        TileInfo tileInfo = obj.GetComponent<TileInfo>();

        for (int i = 0; i < baseTile.Count; i++)
        {
            TileInfo bTile = baseTile[i].GetComponent<TileInfo>();

            if (bTile.tileType == tileInfo.tileType)
            {
                return i;
            }
        }

        return 0;
    }

    private void GenerateMapLogic()
    {

        int numberOfTilesInARow = 0;
        int tileCounter = 0;
        int index = 0;
        int localX = 0;
        int initialReserveIndex = 0; //Cannot be trusted will dupe value if it goes to spawnDown statement
        Vector2 tileLocation = Vector2.zero;
        Vector2 placeHolderPos = Vector2.zero;
        int tileLocationX = 0;
        int tileLocationY = 0;
        Dictionary<int, int> reserve = new Dictionary<int, int>();
        //List<int> borderIndex = new List<int>();


        for (int y = 0; y < height; y++)
        {
            GameObject tile = null;

            for (int x = 0; x < width; x++)
            {

                localX++;
                initialReserveIndex++;


                if (tileCounter >= numberOfTilesInARow)
                {

                    index = Random.Range(0, baseTile.Count);
                    TileInfo candidateTile = baseTile[index].GetComponent<TileInfo>();
                    numberOfTilesInARow = Random.Range(candidateTile.minChance, candidateTile.maxChance);

                    tileCounter = 0;
                }


                if (numberOfTilesInARow != 0)
                {

                    int spawnDown = Random.Range(1, 100);
                    int reserveIndex = initialReserveIndex + (int)width;
                    int tileReserved = 0;

                    //Checkes each line if in keypair match if yes do not continue execution replace baseTile[kepair]
                    if (spawnDown < spawnDownChance && !reserve.ContainsKey(reserveIndex))
                    {
                        reserve.Add(reserveIndex, index);
                        tileCounter++;
                        tileReserved++;
                        initialReserveIndex--;
                        x--;
                    }

                    if (reserve.ContainsKey(initialReserveIndex))
                    {
                        index = reserve[initialReserveIndex];
                        reserve.Remove(initialReserveIndex);
                    }

                    if (tileCounter - tileReserved <= numberOfTilesInARow && localX <= width)
                    {
                        //Check Up and Left generated obj if they are the same then lets make this new tile the same
                        GameObject UpObj = GetObjectFrom(tileLocation, Direction.Up);
                        GameObject LeftObj = GetObjectFrom(tileLocation, Direction.Left);

                        if (UpObj != null && LeftObj != null)
                        {
                            int upIndex = GetBaseTileIndex(UpObj);
                            int leftIndex = GetBaseTileIndex(LeftObj);

                            if (upIndex == leftIndex && Random.Range(1, 100) < homogeneousChance)
                            {
                                index = upIndex;
                            }
                        }

                        tile = Instantiate(baseTile[index], placeHolderTile.transform.position, placeHolderTile.transform.rotation, placeHolderTile.transform.parent);
                        TileInfo tileInfo = tile.GetComponent<TileInfo>();
                        tileLocation = new Vector2(tileLocationX, tileLocationY);
                        tileInfo.tileLocation = tileLocation;
                        tileInfo.Initialize();

                        onGenerateTile(ref tile, placeHolderTile, tileLocation, localX, y);
                        //Debug.Log(tileLocation);
                        generatedTile.Add(tileLocation, tile);
                        tile.SetActive(true);
                        tileLocationX++;
                        tileCounter++;

                    }
                }

                placeHolderPos = placeHolderTile.transform.position;
                placeHolderTile.transform.position = new Vector3(placeHolderPos.x + distanceOfEachTile, placeHolderPos.y, 0);
            }

            localX = 0;
            tileLocationX = 0;
            tileLocationY++;
            onNewLine(localX, y);
            /*if (tile != null)
            {
                //Debug.Log(absoluteIndexCount);
                borderIndex.Add(absoluteIndexCount-1);
                //borderTile.Add(tile);
            }*/
            placeHolderTile.transform.position = new Vector3(originalPos.x, placeHolderPos.y - distanceOfEachTile, 0);
        }

        /*borderTile.Add(generatedTile[0]);
        foreach (int i in borderIndex)
        {
            int ii = i + 1;

            if (ii < generatedTile.Count)
            {
                borderTile.Add(generatedTile[ii]);
            }
        }*/

        placeHolderTile.transform.position = originalPos;
        placeHolderTile.SetActive(false);
        onDoneGenerate();
    }

}
