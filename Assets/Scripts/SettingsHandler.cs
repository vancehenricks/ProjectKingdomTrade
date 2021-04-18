/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, April 2021
 */

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SettingsHandler : MonoBehaviour
{
    public static SettingsHandler init;

    private void Awake()
    {
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
            Debug.LogError(e);
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

        return settingsConfig;
    }

    public SettingsConfig GetDefaults(SettingsConfig settingConfig)
    {
        if (settingConfig.font == null)
        {
            settingConfig.font = TextHandler.init.font.name;
        }

        if (settingConfig.language == null)
        {
            settingConfig.language = LanguageHandler.init.defaultLanguage;
        }

        if (settingConfig.maxFps == 0)
        {
            settingConfig.maxFps = FrameRateHandler.init.maxFps;
        }

        if(settingConfig.vSync == false)
        {
            settingConfig.vSync = FrameRateHandler.init.vSync;
        }

        return settingConfig;
    }
}