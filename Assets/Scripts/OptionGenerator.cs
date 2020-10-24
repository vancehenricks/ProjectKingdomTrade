/* Copyright (C) 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, June 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionGenerator : MonoBehaviour {

    public Image image;
    public List<GameObject> options;
    private Dictionary<string, GameObject> optionDictionary;
    //public static bool blockDisplay;

    public void Initialize ()
    {
        if (optionDictionary != null) return;

        optionDictionary = new Dictionary<string, GameObject>();

        foreach (GameObject option in options)
        {
            option.SetActive(false);
            //image.enabled = false;
            optionDictionary.Add(option.name, option);
        }
	}

    public void Display(TileInfo tileInfo)
    {
        /*if (blockDisplay)
        {
            blockDisplay = false;
            return;
        }*/
        //TileSelected.tileInfo = tileInfo;
        SetActiveAll(false);
        gameObject.SetActive(true);
        string tileType = tileInfo.tileType;

        switch (tileType)
        {
            case "Land":
            case "Grass":
            case "Forest":
            case "Sea":
            case "Mountain":
                optionDictionary["Examine_1"].SetActive(true);
                break;
            case "Town":
                optionDictionary["HireMerchant_1"].SetActive(true);
                optionDictionary["EnlistCivilian_1"].SetActive(true);
                optionDictionary["BuildShip_1"].SetActive(true);
                optionDictionary["SetTradePort_1"].SetActive(true);
                optionDictionary["Examine_1"].SetActive(true);
                break;
            case "Unit":
                optionDictionary["Move_1"].SetActive(true);
                optionDictionary["Attack_1"].SetActive(true);
                optionDictionary["Merge_1"].SetActive(true);
                optionDictionary["Resupply_1"].SetActive(true);
                optionDictionary["Examine_1"].SetActive(true);
                break;
        }
    }

    public void SetActiveAll(bool active)
    {
        foreach (GameObject option in options)
        {
            option.SetActive(active);
        }
    }

    public void ResetAll()
    {
        gameObject.SetActive(false);
    }

}
