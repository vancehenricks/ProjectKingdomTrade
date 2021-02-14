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
            if (option == null) continue; 

            option.SetActive(false);
            optionDictionary.Add(option.name, option);
        }
    }

    public void Display(TileInfo tileInfo)
    {
        SetActiveAll(false);
        gameObject.SetActive(true);

        switch (tileInfo.tileType)
        {
            case "Land":
            case "Grass":
            case "Forest":
            case "Sea":
            case "Mountain":
                Show("Examine_1");
                break;
            case "Town":
                Show("Recruit_1");
                Show("BuildShip_1");
                Show("SetTradePort_1");
                Show("Examine_1");
                break;
            case "Unit":
                switch (tileInfo.subType)
                {
                    case "Merchant":
                        Show("Move_1");
                        Show("Trade_1");
                        Show("EstablishTown_1");
                        Show("Examine_1");
                        break;
                    case "Worker":
                        Show("Move_1");
                        Show("Gather_1");
                        ShowMergeAndSplit(tileInfo);
                        Show("Examine_1");
                        break;
                    case "Infantry":
                    case "Cavalry":
                    case "Archer":
                        Show("Move_1");
                        Show("Attack_1");
                        ShowMergeAndSplit(tileInfo);
                        Show("Examine_1");
                        break;
                    case "Trebuchet":
                        Show("Move_1");
                        Show("Attack_1");
                        Show("Examine_1");
                        break;
                    case "Ship":
                        Show("Move_1");
                        Show("Examine_1");
                        break;
                }
                break;
        }
    }

    private void ShowMergeAndSplit(TileInfo tileInfo)
    {
        List<TileInfo> raycastTile = TileInfoRaycaster.tileInfos;
        List<TileInfo> multiSelect = MultiSelect.selectedTiles;

        if (multiSelect.Count > 1 && raycastTile.Count > 1)
        {
            Show("Merge_1");
        }
        else if ((multiSelect.Count == 1 || raycastTile.Count == 1) && tileInfo.units > 1)
        {
            Show("Split_1");
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
        if (optionDictionary == null) return;

        foreach (GameObject option in optionDictionary.Values)
        {
            option.SetActive(active);
        }
    }

    public void ResetAll()
    {
        gameObject.SetActive(false);
    }

}
