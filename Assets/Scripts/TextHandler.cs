/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, April 2021
 */

using DebugHandler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextHandler : MonoBehaviour
{
    private static TextHandler _init;

    public static TextHandler init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public List<Text> texts;
    public Font font;

    private void Awake()
    {
        init = this;
        font = Resources.GetBuiltinResource<Font>("Arial.ttf");
    }

    public void Load()
    {
        texts.Clear();
        texts.AddRange(Resources.FindObjectsOfTypeAll<Text>());

        CDebug.Log(this,$"Font={font.name}");

        foreach (Text text in texts)
        {
            text.font = font;
            LanguageHandler.init.Text(text);
        }
    }

    public bool SetAsFont(string _font)
    {
        if (_font == "Arial") return false;

        foreach (string fontName in Font.GetOSInstalledFontNames())
        {
            if (fontName == _font)
            {
               font = Font.CreateDynamicFontFromOSFont(_font, 24);
               return true;
            }
        }

        return false;
    }

}
