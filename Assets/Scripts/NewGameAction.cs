﻿/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NewGameAction : MonoBehaviour
{

    public Text kingdomName;
    public Text width;
    public Text height;
    public RectTransform grid;
    public ResetCenter resetCenter;
    public OpenWindow openWindow;
    public CloseWindow closeWindow;
    /*public CloudCycle cloudCycle;*/
    public CloudCycle cloudCycle2;
    public CelestialCycle celestialCycle;
    public Tick tick;

    private int w;
    private int h;

    public void DoAction()
    {

        bool wResult = int.TryParse(width.text, out w);
        bool hResult = int.TryParse(height.text, out h);

        grid.sizeDelta = new Vector2(500f, 500f);

        if (wResult && hResult || width.text == "" && height.text == "")
        {

            if (width.text == "" && height.text == "")
            {
                w = h = 500;
            }

            if (Tools.GetNumberOfTiles(w, h, 25, 25) > Tools.GetNumberOfTiles(1000, 1000, 25, 25))
            {
                ShowMessage show = ShowMessageHandler.init.confirmWindow.SetMessage("Warning",
                    "Map size is greater than 1000x1000. This could cause the game to be unresponsive. Would you like to continue?",
                    "Yes", "No", null, OnResponse);

                if (!show.response) return;
            }

            StartCoroutine(DoNewGameLogic());

        }
        else
        {
            ShowMessageHandler.init.infoWindow.SetMessage("Invalid Input", "Integer values only in map size", "OK", null, null);
        }

    }

    private void OnResponse(bool response)
    {
        if (response)
        {
            StartCoroutine(DoNewGameLogic());
        }
    }

    private IEnumerator DoNewGameLogic()
    {
        closeWindow.DoClose();
        LoadingHandler.init.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        grid.sizeDelta = new Vector2(w, h);
        SyncSize.doSync();
        resetCenter.DoAction();
        LoadingHandler.init.Set(0.3f, "Generating Tiles...");
        yield return new WaitForSeconds(0.5f);
        MapGenerator.init.Initialize();
        LoadingHandler.init.Set(0.5f, "Generating Tiles...");
        yield return new WaitForSeconds(0.5f);
        //cloudCycle.Initialize();
        cloudCycle2.Initialize();
        celestialCycle.Initialize();
        tick.Initialize();

        float speed = ((w / 10) / 10);

        Tick.speed = (int)speed + 30;
        LoadingHandler.init.Set(0.8f, "Generating Clouds...");
        yield return new WaitForSeconds((speed/10)+3f);
        Tick.speed = 1;
        tick.Initialize();
        LoadingHandler.init.Set(1f, "Setting up Time...");
        yield return new WaitForSeconds(1.5f);
        LoadingHandler.init.SetActive(false);
        openWindow.DoOpen();
    }
}
