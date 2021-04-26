/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, April 2021
 */

using DebugHandler;
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
        Debug.unityLogger.filterLogType = (LogType)settingsConfig.logLevel;

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

        return settingsConfig;
    }
}