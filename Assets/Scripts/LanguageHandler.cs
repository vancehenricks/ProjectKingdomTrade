/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, April 2021
 */


using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class LanguageHandler : MonoBehaviour
{
    public static LanguageHandler init;
    public Dictionary<string, string> texts;
    public Dictionary<string, string> language;
    public string _defaultLanguage;

    public string defaultLanguage
    {
        get; private set;
    }

    public string secondaryLanguage;

    private static Regex regex;

    private void Awake()
    {
        init = this;
        defaultLanguage = "English";
        secondaryLanguage = "English";
        texts = new Dictionary<string, string>();
        language = new Dictionary<string, string>();

        if (regex == null)
        {
            regex = new Regex(@"\[(.*)\]", RegexOptions.Compiled);
        }
    }

    public void Load()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Locale");
        string[] includePaths = Directory.GetFiles(path, "*include");
        foreach (string includePath in includePaths)
        {
            string fileName = Path.GetFileName(includePath);
            AddToDictionary(File.ReadAllLines(includePath), fileName, language);
        }

        string defaultFileName = language[defaultLanguage];

        Debug.Log($"Default Language {defaultLanguage} {defaultFileName}");

        AddToDictionary(File.ReadAllLines(Path.Combine(path, defaultFileName)), defaultFileName, texts);

        if (defaultLanguage == secondaryLanguage) return;

        string secondaryFileName = language[secondaryLanguage];

        Debug.Log($"Secondary Language {secondaryLanguage} {secondaryFileName}");

        AddToDictionary(File.ReadAllLines(Path.Combine(path, secondaryFileName)), secondaryFileName, texts);

    }

    public bool Text(Text text, string id = "")
    {
        if (id == "")
        {
            id = text.text;
        }

        string tempId = id;

        if (id.Contains("[") && id.Contains("]"))
        {
            id = regex.Match(id).Groups[1].Value;
        }

        if (!texts.ContainsKey(id))
        {
            text.text = tempId;
            return false;
        }

        text.text = text.text.Replace("[" + id + "]", texts[id]);

        Debug.Log($"{id } = {text.text}");

        return true;
    }

    private void AddToDictionary(string[] lang, string fileName, Dictionary<string, string> _language)
    {
        try
        {
            foreach (string raw in lang)
            {
                if (raw.Contains("//") || raw.Length == 1) continue;

                string[] pairs = raw.Split('=');

                if (pairs[0] != " " && pairs[1] == " ") continue;

                if (_language.ContainsKey(pairs[0]))
                {
                    _language[pairs[0]] = pairs[1];
                }
                else
                {
                    _language.Add(pairs[0], pairs[1]);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            ShowMessageHandler.init.infoWindow.SetMessage("Error - " + fileName,
                e.ToString(), "OK", null, null);
        }
    }
}
