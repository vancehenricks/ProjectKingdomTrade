﻿/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, June 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellInfo : MonoBehaviour
{

    public Image image;
    public Image border;
    public Image select;
    public Text value;
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
        showOptions = _showOptions;
        multiSelect = _multiSelect;
        optionGenerator = generateCells.optionGenerator;
        tileInfo = tile;

        if (tile.units > 0)
        {
            value.text = Tools.ConvertToSymbols(tile.units);
        }

        Image shade = null;

        if (tileInfo.tileEffect != null)
        {
            image.sprite = tileInfo.tileEffect.image.sprite;
        }
        else
        {
            Image img = tileInfo.tileCaller.image.GetComponent<Image>();
            shade = tileInfo.tileCaller.shade.GetComponent<Image>();
            image.sprite = img.sprite;
        }

        if (tileInfo.playerInfo == null)
        {
            border.gameObject.SetActive(false);
            image.color = Color.white;
        }
        else
        {
            border.gameObject.SetActive(true);

            if (shade == null)
            {
                border.color = new Color(tileInfo.playerInfo.color.r, tileInfo.playerInfo.color.g, tileInfo.playerInfo.color.b);
            }
            else
            {
                border.sprite = shade.sprite;
                border.color = new Color(tileInfo.playerInfo.color.r, tileInfo.playerInfo.color.g, tileInfo.playerInfo.color.b, shade.color.a);
            }
        }

        if (showOptions)
        {
            optionGenerator.Initialize();
        }

        gameObject.SetActive(true);
    }

    public void OpenOptions()
    {
        if (MultiSelect.shiftPressed && multiSelect)
        {
            MultiSelect.Add(tileInfo, true);
            isSelectedTwice = false;
        }
        else if (!isSelectedTwice)
        {
            generateCells.ResetSelectedCells();
            generateCells.DisableSelectCells();
            MultiSelect.Clear(true);
            MultiSelect.Add(tileInfo, true);
            isSelectedTwice = true;
        }
        else if (!MultiSelect.shiftPressed)
        {
            MultiSelect.Clear(true);
            generateCells.SelectAll(tileInfo);
            MultiSelect.Relay();
            isSelectedTwice = false;
        }

        select.gameObject.SetActive(true);

        if (showOptions)
        {
            optionGenerator.transform.position = corner.position + offset;
            //generate approriate option where it will only show that is common on all selected tiles
            optionGenerator.Display(tileInfo, GetCommonOptions(MultiSelect.selectedTiles));

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
