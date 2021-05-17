/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, April 2021
 */

using DebugHandler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SettingsHandler : MonoBehaviour
{
    private static SettingsHandler _init;
    public static SettingsHandler init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public CameraDraggableWindow cameraDraggableWindow;
    public ResetCenter resetCenter;
    public HideBorder hideTerritory;
    public CloudCycle cloudCycle;
    public ZoomingWindow zoomingWindow;
    public TimeWindowAction timeWindowAction;

    private void Awake()
    {
        Debug.unityLogger.filterLogType = LogType.Log;
        CDebug.Log(this, "version=" + Application.version, LogType.Warning);
        
        init = this;
    }

    public void Load()
    {
        string settingsPath = Path.Combine(Application.streamingAssetsPath, "settings.json");

        if (!File.Exists(settingsPath))
        {
            Tools.WriteConfig(GetDefaults(new SettingsConfig()), settingsPath);
        }

        string json = File.ReadAllText(settingsPath);
        try
        {
            SettingsConfig settingsConfig = Convert(JsonUtility.FromJson<SettingsConfig>(json));
            Tools.WriteConfig(settingsConfig, settingsPath);
        }
        catch (System.Exception e)
        {
            CDebug.Log(this,e,LogType.Error);
            ShowMessageHandler.init.infoWindow.SetMessage("Json [ERROR] - settings.json",
                e.ToString(), "OK", null, null);
        }
    }

    public SettingsConfig Convert(SettingsConfig settingsConfig)
    {
        settingsConfig = GetDefaults(settingsConfig);

        TextHandler.init.SetAsFont(settingsConfig.font);
        LanguageHandler.init.secondaryLanguage = settingsConfig.language;
        FrameRateHandler.init.maxFps = settingsConfig.maxFps;
        FrameRateHandler.init.vSync = settingsConfig.vSync;
        TileOcclusion.init.overflow = settingsConfig.tileOcclusion;
        Debug.unityLogger.filterLogType = (LogType)settingsConfig.logLevel;
        PathFindingQueue.init.maxQueue = settingsConfig.maxPathFindingQueue;
        PathFindingCache.init.maxCache = settingsConfig.maxPathFindingCache;
        TileInfoRaycaster.init.maxHits = settingsConfig.maxHits;

        hideTerritory.key = (KeyCode)Enum.Parse(typeof(KeyCode), settingsConfig.hideTerritoryKey);
        cloudCycle.hideClouds = (KeyCode)Enum.Parse(typeof(KeyCode), settingsConfig.hideCloudsKey);
        HideObjectHandler.init.hideObjects["hideUI"].key = (KeyCode)Enum.Parse(typeof(KeyCode), settingsConfig.hideUIKey);
        cameraDraggableWindow.isMouseLock = settingsConfig.cursorEdgeMove;
        cameraDraggableWindow.mouseLockKey = (KeyCode)Enum.Parse(typeof(KeyCode), settingsConfig.cursorEdgeMoveKey);
        resetCenter.key = (KeyCode)Enum.Parse(typeof(KeyCode), settingsConfig.centerCameraKey);

        HideObjectHandler.init.hideObjects["showConsole"].key = (KeyCode)Enum.Parse(typeof(KeyCode), settingsConfig.showConsole);
        HideObjectHandler.init.hideObjects["showDebug"].key = (KeyCode)Enum.Parse(typeof(KeyCode), settingsConfig.debugWindowKey);

        zoomingWindow.defaultSpeed = settingsConfig.zoomSpeed;
        zoomingWindow.minScale = settingsConfig.minZoomScale;
        zoomingWindow.maxScale = settingsConfig.maxZoomScale;
        zoomingWindow.maxScaleMultipler = settingsConfig.maxZoomMultiplier;
        timeWindowAction.maxSpeed = settingsConfig.maxTimeSpeed;


        return settingsConfig;
    }

    public SettingsConfig GetDefaults(SettingsConfig settingsConfig)
    {
        if (settingsConfig.font == null)
        {
            settingsConfig.font = TextHandler.init.font.name;
        }

        if (settingsConfig.language == null)
        {
            settingsConfig.language = LanguageHandler.init.defaultLanguage;
        }

        if (settingsConfig.maxFps == 0)
        {
            settingsConfig.maxFps = FrameRateHandler.init.maxFps;
        }

        if(settingsConfig.vSync == false)
        {
            settingsConfig.vSync = FrameRateHandler.init.vSync;
        }

        if (settingsConfig.logLevel == 0)
        {
            settingsConfig.logLevel = (int)LogType.Warning;
        }

        if (settingsConfig.tileOcclusion == 0)
        {
            settingsConfig.tileOcclusion = TileOcclusion.init.overflow;
        }

        if (settingsConfig.maxPathFindingCache == 0)
        {
            settingsConfig.maxPathFindingCache = PathFindingCache.init.maxCache;
        }

        if (settingsConfig.maxPathFindingQueue == 0)
        {
            settingsConfig.maxPathFindingQueue = PathFindingQueue.init.maxQueue;
        }

        if (settingsConfig.maxHits == 0)
        {
            settingsConfig.maxHits = TileInfoRaycaster.init.maxHits;
        }

        if (settingsConfig.hideTerritoryKey == null)
        {
            settingsConfig.hideTerritoryKey = hideTerritory.key.ToString();
        }

        if(settingsConfig.hideCloudsKey == null)
        {
            settingsConfig.hideCloudsKey = cloudCycle.hideClouds.ToString();    
        }

        if (settingsConfig.hideUIKey == null)
        {
            settingsConfig.hideUIKey = HideObjectHandler.init.hideObjects["hideUI"].key.ToString();
        }

        if (settingsConfig.cursorEdgeMove == false)
        {
            settingsConfig.cursorEdgeMove = cameraDraggableWindow.isMouseLock;
        }

        if (settingsConfig.cursorEdgeMoveKey == null)
        {
            settingsConfig.cursorEdgeMoveKey = cameraDraggableWindow.mouseLockKey.ToString();
        }

        if (settingsConfig.centerCameraKey == null)
        {
            settingsConfig.centerCameraKey = resetCenter.key.ToString();
        }

        if (settingsConfig.showConsole == null)
        {
            settingsConfig.showConsole = HideObjectHandler.init.hideObjects["showConsole"].key.ToString();
        }

        if (settingsConfig.debugWindowKey == null)
        {
            settingsConfig.debugWindowKey = HideObjectHandler.init.hideObjects["showDebug"].key.ToString();
        }

        if (settingsConfig.zoomSpeed == 0)
        {
            settingsConfig.zoomSpeed = zoomingWindow.defaultSpeed;
        }

        if (settingsConfig.minZoomScale == 0)
        {
            settingsConfig.minZoomScale = zoomingWindow.minScale;
        }

        if (settingsConfig.maxZoomScale == 0)
        {
            settingsConfig.maxZoomScale = zoomingWindow.maxScale;
        }

        if (settingsConfig.maxZoomMultiplier == 0)
        {
            settingsConfig.maxZoomMultiplier = zoomingWindow.maxScaleMultipler;
        }

        if (settingsConfig.maxTimeSpeed == 0)
        {
            settingsConfig.maxTimeSpeed = timeWindowAction.maxSpeed;
        }

        return settingsConfig;
    }
}