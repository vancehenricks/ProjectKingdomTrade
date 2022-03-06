/* Copyright 2022 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, March 2022
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoTip : MonoBehaviour
{
    private static InfoTip _init;
    public static InfoTip init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public GameObject infoTip;
    public TileInfo currentTile;

    public Vector3 offset;

    private TMP_Text text;
    private Coroutine checkInfo;

    private void Awake()
    {
        init = this;
    }

    public void Initialize()
    {
        text = infoTip.GetComponentInChildren<TMP_Text>();
        checkInfo = StartCoroutine(CheckInfo());
        
    }

    public void Display(bool display, Vector3 pos, string info = null)
    {
        infoTip.transform.position = pos + offset;
        infoTip.SetActive(display);
        text.text = info;
    }

    private IEnumerator CheckInfo()
    {
        while(true) 
        {
            //CDebug.Log(this, "Checking....", LogType.Warning);
            TileInfo newTile = TileInfoRaycaster.init.GetTileInfoFromPos(Input.mousePosition);
            if(currentTile != newTile) 
            {
                if(currentTile != null) 
                {
                    Display(false, Input.mousePosition);
                }

                if(newTile != null) {
                    Display(true, Input.mousePosition, newTile.tileType); //this is temp should display tileName later
                }

                currentTile = newTile;
            }

            yield return null;
        }
    }

    private void OnDestory()
    {
        if(checkInfo != null)
        {
            StopCoroutine(checkInfo);
        }
    }
}
