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

    private void Awake()
    {
        init = this;
        defaultLanguage = "English";
        secondaryLanguage = "English";
        texts = new Dictionary<string, string>();
        language = new Dictionary<string, string>();
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

        AddToDictionary(File.ReadAllLines(Path.Combine(path, "Language", defaultFileName)), defaultFileName, texts);

        if (defaultLanguage == secondaryLanguage) return;

        string secondaryFileName = language[secondaryLanguage];

        Debug.Log($"Secondary Language {secondaryLanguage} {secondaryFileName}");

        AddToDictionary(File.ReadAllLines(Path.Combine(path, "Language", secondaryFileName)), secondaryFileName, texts);

    }

    public bool Text(ref string source, string id = "")
    {
        if (id == "")
        {
            id = source;
        }

        string tempId = id;
        List<string> ids = new List<string>();

        if (id.Contains("[") && id.Contains("]"))
        {
            FormatId(id, ids);
        }

        foreach (string _id in ids)
        {
            if (!texts.ContainsKey(_id))
            {
                source = tempId;
                return false;
            }

            source = source.Replace("[" + _id + "]", texts[_id]);

            Debug.Log($"{_id} = {source}");
        }

        return true;
    }

    public bool Text(Text text, string id = "")
    {
        string t = id == "" ? text.text : id;
        bool result = Text(ref t, id);
        text.text = t;

        return result;
    }

    private void FormatId(string _id, List<string> finalize)
    {
        string[] ids = _id.Split('[');

        foreach (string id in ids)
        {
            if (!id.Contains("]")) continue;
            finalize.Add(id.Substring(0, id.IndexOf(']')));
        }
    }


    private void AddToDictionary(string[] lang, string fileName, Dictionary<string, string> _language)
    {
        try
        {
            string currentId = "";

            foreach (string raw in lang)
            {
                if (raw.Contains("//") || raw.Length == 1) continue;

                string[] pairs = raw.Split(new char[]{'='}, 2);

                if (pairs[0] != " " && pairs[1] == " " && pairs[1] == "") continue;

                if (pairs[0] == "" && currentId != "")
                {
                    _language[currentId] += pairs[1];
                    continue;
                }
                else
                {
                    currentId = pairs[0];
                }

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
            ShowMessageHandler.init.infoWindow.SetMessage("[ERROR] - " + fileName,
                e.ToString(), "[OK]", null, null);
        }
    }
}
