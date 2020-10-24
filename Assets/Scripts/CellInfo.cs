/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, June 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellInfo : MonoBehaviour {

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
            shade = tileInfo.tileCaller.shade.GetComponent <Image>();
            image.sprite = img.sprite;
        }

        if (tileInfo.color == Color.white)
        {
            border.gameObject.SetActive(false);
            image.color = new Color(tileInfo.color.r, tileInfo.color.g, tileInfo.color.b);
        }
        else
        {
            border.gameObject.SetActive(true);

            if (shade == null)
            {
                border.color = new Color(tileInfo.color.r, tileInfo.color.g, tileInfo.color.b);
            }
            else
            {
                border.sprite = shade.sprite;
                border.color = new Color(tileInfo.color.r, tileInfo.color.g, tileInfo.color.b, shade.color.a);
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
            MultiSelect.selectedTiles.Add(tileInfo);
            MultiSelect.Relay();
            isSelectedTwice = false;
        }
        else if (!isSelectedTwice)
        {
            generateCells.ResetSelectedCells();
            generateCells.DisableSelectCells();
            MultiSelect.selectedTiles.Clear();
            MultiSelect.selectedTiles.Add(tileInfo);
            MultiSelect.Relay();
            isSelectedTwice = true;
        }
        else
        {
            generateCells.SelectAll(tileInfo);
            MultiSelect.Relay();
            isSelectedTwice = false;
        }

        select.gameObject.SetActive(true);

        if (showOptions)
        {
            optionGenerator.transform.position = corner.position + offset;
            optionGenerator.Display(tileInfo);
        }

    }
}
