﻿/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, July 2019
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitDisplay : MonoBehaviour
{


    public bool stop;
    public SyncIcon syncIconBase;
    public float xLevel;
    public float yLevel;
    public float zLevel;

    private SyncIcon syncIcon;
    private TextMeshProUGUI unitText;
    private TextMeshProUGUI multiUnitText;
    private TileInfo tile;

    private enum Obj
    {
       Morale = 0, Wall, Unit, Aqueduct, MultiUnit
    }

    private void Awake()
    {
        tile = GetComponent<TileInfo>();
    }

    private void Start()
    {
        if (!stop)
        {
            Initialize();
        }
    }

    public void Initialize()
    {
        syncIcon = Instantiate(syncIconBase, syncIconBase.transform.parent);
        unitText = syncIcon.genericObjectHolder.GetComponent<TextMeshProUGUI>((int)Obj.Unit);
        multiUnitText = syncIcon.genericObjectHolder.GetComponent<TextMeshProUGUI>((int)Obj.MultiUnit);
        syncIcon.Initialize(tile, xLevel, yLevel, zLevel);
        syncIcon.gameObject.SetActive(true);
        syncIcon.Sync(true);

        if (tile.tileType == "Unit")
        {
            StartCoroutine(Sync(UnitUpdate));
        }
        else
        {
            StartCoroutine(Sync(TileUpdate));
        }
    }

    public void Sync()
    {
        if (syncIcon == null) return;

        if (!tile.tileEffect.image.activeSelf || (tile.tileType != "Unit" && tile.standingTiles.Count > 0))
        {
            StopAllCoroutines();
            syncIcon.Sync(false);
            syncIcon.SetActive(false);
        }
        else
        {
            StopAllCoroutines();
            if (tile.tileType == "Unit")
            {
                StartCoroutine(Sync(UnitUpdate));
            }
            else
            {
                StartCoroutine(Sync(TileUpdate));
            }

            syncIcon.Sync(true);
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private IEnumerator Sync(System.Action action)
    {
        while (true)
        {
            action();
            yield return null;
        }
    }

    private void TileUpdate()
    {
        if (tile.unit == 0) return;

        unitText.text = Tools.ConvertToSymbols(tile.unit);
        multiUnitText.text = "";
    }

    private void UnitUpdate()
    {
        UnitInfo unit = tile as UnitInfo;

        if (unit == null) return;
        if (unit.standingTile == null) return;

        int units = unit.standingTile.unit;
        int multiUnit = unit.standingTile.standingTiles.Count;

        if (unit.standingTile.unit > 0)
        {
            multiUnit += 1;
        }

        foreach (TileInfo standingTile in unit.standingTile.standingTiles)
        {
            units += standingTile.unit;
        }

        if (units == 0) return;

        unitText.text = Tools.ConvertToSymbols(units);

        if (multiUnit > 1)
        {
            multiUnitText.text = Tools.ConvertToSymbols(multiUnit);
        }
        else
        {
            multiUnitText.text = "";
        }
    }
}
