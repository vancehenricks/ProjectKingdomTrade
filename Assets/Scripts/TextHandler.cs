/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, April 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextHandler : MonoBehaviour
{
    public static TextHandler init;
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

        Debug.Log($"Font {font.name}");

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
