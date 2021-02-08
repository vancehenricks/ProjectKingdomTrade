/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TownGenerator : MonoBehaviour
{
    private static TownGenerator _init;

    public static TownGenerator init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public List<GameObject> baseTile;
    public bool allowAdvanceTowns;
    public int maxAmountOfTowns;
    public int distanceOfEachTowns;
    public int spawnChance;
    public PlayerInfo basePlayerInfo;

    public List<TileInfo> allowedTiles;

    private int nTowns;
    private int xCounter;
    private int yCounter;
    public Dictionary<Vector2, GameObject> generatedTowns;

    private void Start()
    {
        _init = this;
        MapGenerator.init.onInitialize = MapGenerator.init.onInitialize + Initialize;
        generatedTowns = new Dictionary<Vector2, GameObject>();
    }

    public void Initialize()
    {
        nTowns = 0;
        MapGenerator.init.onGenerateTile += OnGenerateTile;
        MapGenerator.init.onNewLine += OnNewLine;
        MapGenerator.init.onDoneGenerate += OnDoneGenerate;
        generatedTowns.Clear();
    }

    public void OnGenerateTile(ref GameObject tile, GameObject placeHolderTile, Vector2 location, int x, int y)
    {

        int i = 0;
        TileInfo parentTileInfo = tile.GetComponent<TileInfo>();

        if (allowAdvanceTowns)
        {
            i = Random.Range(0, baseTile.Count);
        }

        if (spawnChance < Random.Range(0, 100))
        {
            return;
        }

        xCounter++;

        if (nTowns < maxAmountOfTowns && xCounter > distanceOfEachTowns && yCounter >= distanceOfEachTowns)
        {

            bool allowed = false;

            foreach (TileInfo t in allowedTiles)
            {
                if (parentTileInfo.tileType == t.tileType)
                {
                    allowed = true;
                    break;
                }
            }

            if (allowed)
            {
                Destroy(tile);
                tile = Instantiate(baseTile[i], placeHolderTile.transform.position, placeHolderTile.transform.rotation, placeHolderTile.transform.parent);
                TownInfo townInfo = tile.GetComponent<TownInfo>();
                townInfo.tileLocation = location;
                townInfo.Initialize();
                townInfo.playerInfo = PlayerList.init.Instantiate();
                generatedTowns.Add(location, tile);
                Debug.Log("TILENAME: " + townInfo.tileType);
                xCounter = 0;
                nTowns++;
            }
        }

    }

    public void OnNewLine(int x, int y)
    {
        if (yCounter >= distanceOfEachTowns)
        {
            yCounter = 0;
        }
        else
        {
            yCounter++;
        }
    }

    public void OnDoneGenerate()
    {
        //Loop through the generated towns and activate border

        List<Color> colors = new List<Color>();
        Color a = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f); //This is temporary
        colors.Add(a);

        foreach (GameObject obj in generatedTowns.Values)
        {
            obj.GetComponent<TileCaller>().border.SetActive(true);
            Color temp = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f); //This is temporary


            if (colors.Exists(c => c != temp))
            {
                colors.Add(temp);
                obj.GetComponent<TileInfo>().playerInfo.color = temp;
            }
        }
    }
}
