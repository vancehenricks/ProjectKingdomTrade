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

public class TownGenerator : MonoBehaviour
{
    public PlayerInfo basePlayerInfo;

    private void Start()
    {
        MapGenerator.init.Add(Generate, 8f);
    }

    public void Generate()
    {
        Debug.Log("Town Generator");
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
