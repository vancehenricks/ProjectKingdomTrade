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
    public CloudCycle cloudCycle;
    public CelestialCycle celestialCycle;
    public ZoomingWindow zoomingWindow;
    //public GameObject unitIndicatorBase;
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
        if (IsNormalize(width, out w, 20) && IsNormalize(height, out h, 20))
        {
            float tileSize = Tools.tileSize;
            if (Tools.GetNumberOfTiles(w, h) > Tools.GetNumberOfTiles(40, 40))
            {
                //unitIndicatorBase.SetActive(false); //setting the base as false enables occlusion likely a bug will take advantage for now

                ShowMessage show = ShowMessageHandler.init.confirmWindow.SetMessage("[WARNING]",
                    "[MAP-SIZE-GREATER-THAN-40X40] [THIS-COULD-CAUSE-THE-GAME-TO-BE-UNRESPONSIVE] [WOULD-YOU-LIKE-TO-CONTINUE]",
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
        TileColliderHandler.init.Initialize();
        yield return new WaitForSeconds(0.5f);
        grid.sizeDelta = new Vector2(w * Tools.tileSize, h * Tools.tileSize);
        SyncSize.init.doSync();
        ResetCenter.init.Initialize();
        LoadingHandler.init.Set(0.3f, "[GENERATING-TILES]");
        yield return new WaitForSeconds(0.5f);
        MapGenerator.init.Initialize(x,y,s);
        LoadingHandler.init.Set(0.5f, "[GENERATING-TILES]");
        yield return new WaitForSeconds(0.5f);
        LoadingHandler.init.Set(0.8f, "[SETTING-UP-TIME]");
        yield return new WaitForSeconds(0.5f);
        tick.Initialize();
        LoadingHandler.init.Set(1f, "[GENERATING-CLOUDS]");
        yield return new WaitForSeconds(0.5f);
        cloudCycle.Initialize();
        celestialCycle.Initialize();
        zoomingWindow.Initialize();
        TileOcclusion.init.Initialize();
        UnitOcclusion.init.Initialize();
        PathFindingQueue.init.Initialize();
        InfoTip.init.Initialize();
        //TimeWindowAction.init.Initialize();
        LoadingHandler.init.SetActive(false);
        openWindow.DoOpen();
    }
}
