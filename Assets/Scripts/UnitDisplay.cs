/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, July 2019
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitDisplay : MonoBehaviour
{

    public TileInfo tile;
    public GameObject indicator;

    public float xLevel;
    public float yLevel;
    public float zLevel;

    public GameObject instance;

    private SyncIcon syncIcon;
    private GenericObjectHolder genericObjectHolder;
    private Text text;
    private Image background;
    private int unitTrack;

    private enum Obj
    {
        Panel = 0, Value
    }

    private void Start()
    {
        //Debug.Log("STARTING!!!");

        instance = Instantiate(indicator);
        instance.transform.SetParent(indicator.transform.parent);
        genericObjectHolder = instance.GetComponent<GenericObjectHolder>();

        syncIcon = instance.GetComponent<SyncIcon>();
        text = genericObjectHolder.GetComponent<Text>((int)Obj.Value);
        syncIcon.syncThroughSibling = true;
        syncIcon.Initialize(tile, xLevel, yLevel, zLevel);
        background = genericObjectHolder.GetComponent<Image>((int)Obj.Panel);
        instance.gameObject.SetActive(true);
        syncIcon.Sync(true);
    }

    private void Update()
    {
        if (instance == null || unitTrack == tile.unit) return;

        //Debug.Log("UPDATING text");

        text.text = Tools.ConvertToSymbols(tile.unit);
        unitTrack = tile.unit;
        background.color = new Color(tile.playerInfo.color.r, tile.playerInfo.color.g, tile.playerInfo.color.b, background.color.a);
    }
}
