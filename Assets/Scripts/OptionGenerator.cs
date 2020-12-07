/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, June 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OptionGenerator : MonoBehaviour
{

    public Image image;
    public List<GameObject> options;
    private Dictionary<string, GameObject> optionDictionary;
    //public static bool blockDisplay;

    public void Initialize()
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
                Show("Examine_1");
                break;
            case "Town":
                Show("HireMerchant_1");
                Show("EnlistCivilian_1");
                Show("BuildShip_1");
                Show("SetTradePort_1");
                Show("Examine_1");
                break;
            case "Unit":
                Show("Move_1");
                Show("Attack_1");
                if (MultiSelect.Count() > 1)
                {
                    Show("Merge_1");
                }
                else if (MultiSelect.GetSelectedTiles()[0].units > 1)
                {
                    Show("Split_1");
                }
                Show("Resupply_1");
                Show("Examine_1");
                break;
        }
    }

    private void Show(string name)
    {
        GameObject obj = optionDictionary[name];
        obj.transform.SetAsLastSibling();
        obj.SetActive(true);
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
