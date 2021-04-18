/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
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
    public Text xOffset;
    public Text yOffset;
    public Text scale;
    public SeedPlaceHolder seedPlaceHolder;
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
    private float x;
    private float y;
    private float s;

    public void DoAction()
    {

        if (CheckSeed() && CheckMapSize())
        {
            StartCoroutine(DoNewGameLogic());
        }

    }

    private bool CheckSeed()
    {
        bool xResult = float.TryParse(xOffset.text, out x);
        bool yResult = float.TryParse(yOffset.text, out y);
        bool sResult = float.TryParse(scale.text, out s);

        if (xResult && yResult && sResult || xOffset.text == "" && yOffset.text == "" && scale.text == "")
        {
            if (xOffset.text == "" && yOffset.text == "" && scale.text == "")
            {
                x = seedPlaceHolder.xOffset;
                y = seedPlaceHolder.yOffset;
                s = seedPlaceHolder.scale;
            }

            return true;
        }
        else
        {
            ShowMessageHandler.init.infoWindow.SetMessage("[INVALID-INPUT]", "[FLOAT-VALUES-ONLY-IN-SEED]", "[OK]", null, null);
            return false;
        }
    }

    private bool CheckMapSize()
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
                ShowMessage show = ShowMessageHandler.init.confirmWindow.SetMessage("[WARNING]",
                    "[MAP-SIZE-GREATER-THAN-1000X1000] [THIS-COULD-CAUSE-THE-GAME-TO-BE-UNRESPONSIVE] [WOULD-YOU-LIKE-TO-CONTINUE]",
                    "[YES]", "[NO]", null, OnResponse);

                if (!show.response) return false;
            }

            return true;

        }
        else
        {
            ShowMessageHandler.init.infoWindow.SetMessage("[INVALID-INPUT]", "[INTEGER-VALUES-ONLY-IN-MAP-SIZE]", "[OK]", null, null);
            return false;
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
        LoadingHandler.init.Set(0.3f, "[GENERATING-TILES]");
        yield return new WaitForSeconds(0.5f);
        MapGenerator.init.Initialize(x,y,s);
        LoadingHandler.init.Set(0.5f, "[GENERATING-TILES]");
        yield return new WaitForSeconds(0.5f);
        //cloudCycle.Initialize();
        cloudCycle2.Initialize();
        celestialCycle.Initialize();
        tick.Initialize();

        float speed = ((w / 10) / 10);

        Tick.speed = (int)speed + 30;
        LoadingHandler.init.Set(0.8f, "[GENERATING-CLOUDS]");
        yield return new WaitForSeconds((speed/10)+3f);
        Tick.speed = 1;
        tick.Initialize();
        LoadingHandler.init.Set(1f, "[SETTING-UP-TIME]");
        yield return new WaitForSeconds(1.5f);
        LoadingHandler.init.SetActive(false);
        openWindow.DoOpen();
    }
}
