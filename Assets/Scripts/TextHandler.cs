/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, April 2021
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///<summary>
///Handle Text.<br/>
///</summary>
public class TextHandler : MonoBehaviour
{
    private static TextHandler _init;

    public static TextHandler init
    {
        get { return _init; }
        private set { _init = value; }
    }

    private List<Text> texts;
    public Font font;

    private void Awake()
    {
        texts = new List<Text>();
        font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        init = this;
    }

    ///<summary>
    ///Load text on every object in the game.<br/>
    ///</summary>
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

    ///<summary>
    ///Set game font.<br/>
    ///_font Font to set.<br/>
    ///Returns a true if it found a font installed in the system, otherwise return false.<br/>
    ///</summary>
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
