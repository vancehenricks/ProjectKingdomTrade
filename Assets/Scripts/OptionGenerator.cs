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
    public Dictionary<string, GameObject> optionDictionary;

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

    public void Display(TileInfo tileInfo, List<string> options = null)
    {
        SetActiveAll(false);
        gameObject.SetActive(true);

        if (options == null)
        {
            options = tileInfo.options;
        }

        foreach (string option in options)
        {
            Show(option, tileInfo);
        }
    }

    private void Show(string name, TileInfo tileInfo)
    {
        List<TileInfo> raycastTile = TileInfoRaycaster.init.tileInfos;
        List<TileInfo> multiSelect = MultiSelect.init.selectedTiles;

        string[] n = name.Split('_');
        name = n[0] + "_" + n[1];

        switch (name)
        {
            case "Merge_1":
                if ((multiSelect.Count == 1 || raycastTile.Count == 1) && tileInfo.units > 1)
                {
                    return;
                }
                break;
            case "Split_1":
                if (multiSelect.Count > 1 && raycastTile.Count > 1)
                {
                    return;
                }
                break;
        }

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
}
