﻿/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, June 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CellInfo : MonoBehaviour
{

    public Image image;
    public Image border;
    public Image select;
    public TextMeshProUGUI unit;
    public TileInfo tileInfo;
    public GenerateCells generateCells;
    public Transform corner;
    public Vector3 offset;
    public bool showOptions;
    public bool multiSelect;
    public bool isSelectedTwice;

    private OptionGenerator optionGenerator;

    public void Initialize(TileInfo tile, bool _showOptions, bool _multiSelect)
    {
        name += "_" + tile.tileId;
        showOptions = _showOptions;
        multiSelect = _multiSelect;
        optionGenerator = generateCells.optionGenerator;
        tileInfo = tile;

        if (tile.unit > 0)
        {
            unit.text = Tools.ConvertToSymbols(tile.unit);
        }
        
        image.sprite = tileInfo.sprite;
        border.sprite = tileInfo.tileEffect.shadeImage.sprite;
        border.color = tileInfo.tileEffect.shadeImage.color;

        if (showOptions)
        {
            optionGenerator.Initialize();
        }

        gameObject.SetActive(true);
    }

    public void OpenOptions()
    {
        if (MultiSelect.init.shiftPressed && multiSelect)
        {
            MultiSelect.init.Add(tileInfo, true);
            isSelectedTwice = false;
        }
        else if (!isSelectedTwice)
        {
            generateCells.ResetSelectedCells();
            generateCells.DisableSelectCells();
            MultiSelect.init.Clear(true);
            MultiSelect.init.Add(tileInfo, true);
            isSelectedTwice = true;
        }
        else if (!MultiSelect.init.shiftPressed)
        {
            MultiSelect.init.Clear(true);
            generateCells.SelectAll(tileInfo);
            MultiSelect.init.Relay();
            isSelectedTwice = false;
        }

        select.gameObject.SetActive(true);

        if (showOptions)
        {
            optionGenerator.transform.position = corner.position + offset;
            //generate approriate option where it will only show that is common on all selected tiles
            optionGenerator.Display(tileInfo, GetCommonOptions(new List<TileInfo>(MultiSelect.init.selectedTiles)));

        }

    }

    private List<string> GetCommonOptions(List<TileInfo> tileInfos)
    {
        if (tileInfos.Count <= 1) return null;

        TileInfo selectedTile = tileInfos[tileInfos.Count - 1];

        Dictionary<string, int> count = new Dictionary<string, int>();

        for (int i = 0; i < tileInfos.Count-1;i++)
        {
            foreach (string option in tileInfos[i].options)
            {
                if(selectedTile.options.Contains(option))
                {
                    if (count.ContainsKey(option))
                    {
                        count[option]++;
                    }
                    else
                    {
                        count.Add(option, 1);
                    }
                }
            }
        }

        List<string> final = new List<string>();

        foreach (string option in count.Keys)
        {
            if (count[option] == tileInfos.Count-1)
            {
                final.Add(option);
            }
        }

        return final;
    }
}
