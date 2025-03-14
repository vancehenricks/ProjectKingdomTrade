﻿/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, June 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateCells : MonoBehaviour
{

    public GridLayoutGroup gridLayoutGroup;
    public OptionGenerator optionGenerator;
    public GameObject content;
    public GameObject baseCell;
    public Image background;
    public int maxConstraintCount;
    public bool showOptions;
    public bool whiteList;
    public bool multiSelect;
    public List<TileInfo> include;


    //public int maxCell;

    //private int maxCellCount;
    // private int currentIndex;

    public List<GameObject> cells;

    public void Initialize()
    {
        include = new List<TileInfo>();
        cells = new List<GameObject>();
        gridLayoutGroup.constraintCount = 1;
    }

    public void Display()
    {
        Display(TileInfoRaycaster.init.tileInfos);
    }

    public void Display(List<TileInfo> _tiles)
    {

        DeleteAllCells();
        //maxCellCount = 0;
        //currentIndex = 0;
        List<TileInfo> tiles = whiteList ? SortTileType(Tools.WhiteListTileType(_tiles, include)) : SortTileType(_tiles);

        if (tiles.Count == 0)
        {
            background.enabled = false;
        }
        else
        {
            background.enabled = true;
        }

        if (tiles.Count > maxConstraintCount)
        {
            gridLayoutGroup.constraintCount = maxConstraintCount;
        }
        else
        {
            gridLayoutGroup.constraintCount = tiles.Count;
        }

        for (int i = 0; i < tiles.Count; i++)
        {
            //maxCellCount++;
            //currentIndex = i;
            GameObject cell = Instantiate(baseCell);
            cell.name = baseCell.name;
            cell.GetComponent<CellInfo>().Initialize(tiles[i], showOptions, multiSelect);
            cell.transform.SetParent(content.transform);
            cells.Add(cell);

            /*if (maxCellCount <= maxCell)
            {
                cell.SetActive(true);
            }*/
        }
    }

    public void ResetSelectedCells()
    {
        foreach (GameObject cell in cells)
        {
            CellInfo cellInfo = cell.GetComponent<CellInfo>();
            cellInfo.isSelectedTwice = false;
        }
    }

    public void SelectAll(TileInfo tile)
    {
        MultiSelect.init.Add(tile);
        foreach (GameObject cell in cells)
        {
            CellInfo cellInfo = cell.GetComponent<CellInfo>();

            if (cellInfo.tileInfo.tileType == tile.tileType 
                && cellInfo.tileInfo.subType == tile.subType
                && cellInfo.tileInfo.tileId != tile.tileId)
            {
                cellInfo.select.gameObject.SetActive(true);
                MultiSelect.init.Add(cellInfo.tileInfo);
            }
        }
    }

    private void DeleteAllCells()
    {
        foreach (GameObject cell in cells)
        {
            Destroy(cell);
        }
        cells.Clear();
    }

    public void DisableSelectCells()
    {
        foreach (var cell in cells)
        {
            Image select = cell.GetComponent<CellInfo>().select;
            select.gameObject.SetActive(false);
        }
    }

    private List<TileInfo> SortTileType(List<TileInfo> tileInfos)
    {
        List<TileInfo> tileOnly = new List<TileInfo>();
        List<TileInfo> unitOnly = new List<TileInfo>(tileInfos);

        foreach(TileInfo tile in tileInfos)
        {
            if(tile.tileType != "Unit")
            {
                tileOnly.Add(tile);
                unitOnly.Remove(tile);
            }
        }

        tileOnly.AddRange(unitOnly);
        
        return tileOnly;
    }

    /*public void DoButtonDown()
    {
        //maxCellCount = 0;
        //currentIndex += maxCell;
        //DeleteAllCells();
    }

    public void DoButtonUp()
    {
        //maxCellCount = 0;
        //currentIndex -= maxCell;
        //DeleteAllCells();
    }*/

}
