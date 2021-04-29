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
using System.Linq;
using DebugHandler;

public class TownGenerator : MonoBehaviour
{
    private static TownGenerator _init;
    public static TownGenerator init
    {
        get { return _init; }
        private set { _init = value; }
    }

    private void Awake()
    {
        init = this;
    }

    private void Start()
    {
        MapGenerator.init.Add(Generate, 8f);
    }

    private void Generate()
    {
        CDebug.Log(this, "Generating Town");

        List<TileInfo> baseTowns = TileConfigHandler.init.baseTowns.Values.ToList<TileInfo>();
        List<TileInfo> generatedTiles = TileList.init.generatedTiles.Values.ToList<TileInfo>();

        FoilageGenerator.init.Generate(baseTowns, generatedTiles, () => {
            return TileList.init.generatedTowns.Values.ToList<TileInfo>();
            }, true);

            //do same thing as foilagegenerator already does but also check for distance

            //Loop through the generated towns and activate border

            /*List<Color> colors = new List<Color>();
            Color a = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f); //This is temporary
            colors.Add(a);

            foreach (TileInfo tile in TileList.generatedTowns.Values)
            {
                tile.tileCaller.border.SetActive(true);
                Color temp = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f); //This is temporary


                if (colors.Exists(c => c != temp))
                {
                    colors.Add(temp);
                    tile.playerInfo.color = temp;
                }
            }*/
    }
}
