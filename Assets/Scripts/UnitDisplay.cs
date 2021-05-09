/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
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

    private Coroutine unitUpdate;
    private Coroutine tileUpdate;

    private enum Obj
    {
       Morale = 0, Wall, Aqueduct, Unit=0, MultiUnit
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
        unitText = syncIcon.genericObjectHolder.texts[(int)Obj.Unit];
        multiUnitText = syncIcon.genericObjectHolder.texts[(int)Obj.MultiUnit];
        syncIcon.Initialize(tile, xLevel, yLevel, zLevel);
        syncIcon.gameObject.SetActive(true);
        syncIcon.Sync(true);

        if (tile.tileType == "Unit")
        {
            unitUpdate = StartCoroutine(Sync(UnitUpdate));
        }
        else
        {
            tileUpdate = StartCoroutine(Sync(TileUpdate));
        }
    }

    public void Sync()
    {
        if (syncIcon == null) return;

        if (!tile.tileEffect.image.activeSelf || (tile.tileType != "Unit" && tile.standingTiles.Count > 0))
        {
            StopCoroutine();
            syncIcon.Sync(false);
            syncIcon.SetActive(false);
        }
        else
        {
            StopCoroutine();
            syncIcon.Sync(true);
            if (tile.tileType == "Unit")
            {
                unitUpdate = StartCoroutine(Sync(UnitUpdate));
            }
            else
            {
                tileUpdate = StartCoroutine(Sync(TileUpdate));
            }
        }
    }

    private void OnDestroy()
    {
        StopCoroutine();

        if (syncIcon != null)
        {
            syncIcon.Destroy();
        }
    }

    private void StopCoroutine()
    {
        if (unitUpdate != null)
        {
            StopCoroutine(unitUpdate);
        }

        if (tileUpdate != null)
        {
            StopCoroutine(tileUpdate);
        }
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
