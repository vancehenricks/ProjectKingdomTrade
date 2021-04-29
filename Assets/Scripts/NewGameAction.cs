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
    public GameObject unitIndicatorBase;
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
        if (IsNormalize(xOffset, out x, seedPlaceHolder.xOffset) && IsNormalize(yOffset, out y, seedPlaceHolder.yOffset)
            && IsNormalize(scale, out s, seedPlaceHolder.scale))
        {
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
        if (IsNormalize(width, out w, 500) && IsNormalize(height, out h, 500))
        {
            float tileSize = Tools.tileSize;
            if (Tools.GetNumberOfTiles(w, h, tileSize, tileSize) > Tools.GetNumberOfTiles(1000, 1000, tileSize, tileSize))
            {
                unitIndicatorBase.SetActive(false); //setting the base as false enables occlusion likely a bug will take advantage for now

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

    private bool IsNormalize(Text text, out float value, float fallback)
    {
            if (float.TryParse(text.text, out value))
            {
                return true;
            }
            else if (text.text == "")
            {
                value = fallback;
                return true;
            }

        return false;
    }

    private bool IsNormalize(Text text, out int value, int fallback)
    {
        if (int.TryParse(text.text, out value))
        {
            return true;
        }
        else if (text.text == "")
        {
            value = fallback;
            return true;
        }

        return false;
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
        SyncSize.init.doSync();
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

        Tick.init.speed = (int)speed + 30;
        LoadingHandler.init.Set(0.8f, "[GENERATING-CLOUDS]");
        yield return new WaitForSeconds((speed/10)+3f);
        Tick.init.speed = 1;
        tick.Initialize();
        LoadingHandler.init.Set(1f, "[SETTING-UP-TIME]");
        yield return new WaitForSeconds(1.5f);
        LoadingHandler.init.SetActive(false);
        openWindow.DoOpen();
    }
}
