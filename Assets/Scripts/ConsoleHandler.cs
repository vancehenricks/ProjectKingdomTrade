/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, March 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using System.Linq;

public class ConsoleHandler : MonoBehaviour
{
    private static ConsoleHandler _init;

    public static ConsoleHandler init
    {
        get { return _init; }
        private set { _init = value; }
    }

    //public delegate void Initialize();
    //public static Initialize initialize;

    public delegate void OnConsoleEvent(string command);
    public static OnConsoleEvent onConsoleEvent;

    public GameObject window;
    public Text console;
    public InputFieldOverride command;
    public Scrollbar scrollBar;
    public RectTransform context;

    public int maxNumberOfLines;
    public float textHeight;

    public string previousCommand;

    private static int index;
    private static Dictionary<string, Dictionary<string, string>> commandList;
    private static string cacheInput;
    private static List<string> cacheConsole;
    private static List<string> cacheCommands;
    private static int numberOfLines;
    private static int remainOnIndex;

    private void Awake()
    {
        if (commandList == null)
        {
            index = 0;

            commandList = new Dictionary<string, Dictionary<string, string>>();
            cacheCommands = new List<string>();
            cacheConsole = new List<string>();

            AddCache("cancel");
            AddCache("help");
            AddCache("clear");
            commandList.Add("cancel", new Dictionary<string, string>());
            commandList.Add("help", new Dictionary<string, string>());
            commandList.Add("clear", new Dictionary<string, string>());

            init = this;

           // initialize();
        }
        else
        {
            AddLine("Queued commands cancelled!");
            command.text = cacheInput;
            AddLines(cacheConsole.ToArray(), false);
            init = this;
        }
    }

    private void OnDestroy()
    {
        cacheInput = command.text;
        //initialize = null;
        onConsoleEvent = null;
    }

    private void Update()
    {
        if (OverrideGetKeyUp(KeyCode.UpArrow))
        {
            Debug.Log("UP");
            IncrementIndex(1);
            DisplayCache();
        }

        if (OverrideGetKeyUp(KeyCode.DownArrow))
        {
            Debug.Log("DOWN");
            IncrementIndex(-1);
            DisplayCache();
        }

        if (OverrideGetKeyUp(KeyCode.Tab))
        {
            string[] substrings = command.text.Split(' ');

            if (substrings.Length == 0) return;

            if (commandList.ContainsKey(substrings[0]))
            {
                if (substrings.Length > 1)
                {
                    AutoFill(substrings, commandList[substrings[0]].Keys, commandList[substrings[0]].Values);
                }
                DisplaySubCommands(substrings[0]);
            }
            else
            {
                if (substrings.Length == 1)
                {
                    AutoFill(substrings, commandList.Keys);
                }
                DisplayCommands();
            }
        }
    }

    public void NextLine()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            previousCommand = command.text;

            command.ActivateInputField();
            if (command.text == "") return;
            AddLine(">" + command.text);

            if (command.text == "clear")
            {
                Clear();
            }
            else if (command.text == "help")
            {
                DisplayCommands();
            }
            else if (command.text == "cancel")
            {
                foreach (var cmd in commandList)
                {
                    onConsoleEvent(cmd.Key + " cancel");
                }
            }
            else if (onConsoleEvent != null)
            {
                onConsoleEvent(command.text);
            }

            remainOnIndex = 0;

            command.text = "";
        }
        else
        {
            command.DeactivateInputField();
        }
    }

    public void DisplayCommands()
    {
        AddLine("Commands:");
        foreach (var cmd in commandList)
        {
            AddLine("\t" + cmd.Key);
        }
    }

    public void DisplaySubCommands(string text)
    {
        Dictionary<string, string> subCommands = commandList[text];
        AddLine("Command:");
        AddLine("\t" + text);
        AddLine("Parameters:");
        foreach (var subCommand in subCommands)
        {
            if (subCommand.Value != "")
            {
                AddLine(string.Format("\t {0}:{1}", subCommand.Key, subCommand.Value));
            }
            else
            {
                AddLine("\t" + subCommand.Key);
            }
        }
    }

    public void AddCache(string text)
    {
        if (!cacheCommands.Contains(text))
        {
            cacheCommands.Add(text);
            index = cacheCommands.Count - 1;
        }
        else
        {
            index = cacheCommands.IndexOf(text);
        }
    }

    public void AddCommand(string cmd, Dictionary<string, string> subCmds)
    {
        if (!commandList.ContainsKey(cmd))
        {
            commandList.Add(cmd, subCmds);
        }
    }

    private bool OverrideGetKeyUp(KeyCode key)
    {
        if (InputOverride.GetKeyUp(key, command.gameObject, true))
        {
            return true;
        }

        return false;
    }

    private bool OverrideGetKeyDown(KeyCode key)
    {
        if (InputOverride.GetKeyDown(key, command.gameObject, true))
        {
            return true;
        }

        return false;
    }

    private IEnumerator ScrollToZero()
    {
        for (int i = 0; i < 3; i++)
        {
            scrollBar.value = 0f;
            yield return null;
        }
    }

    public void AddLine(string line, bool record = true)
    {
        if (++numberOfLines > maxNumberOfLines)
        {
            Clear();
        }

        console.text += line + Environment.NewLine;
        if (record)
        {
            cacheConsole.Add(line);
        }
        context.sizeDelta = new Vector2(0f, context.rect.height + textHeight);
        ScrollZero();
    }

    public void AddLines(string[] lines, bool record = true)
    {
        foreach (string line in lines)
        {
            AddLine(line, record);
        }
    }

    public void Clear()
    {
        console.text = "";
        cacheConsole.Clear();
        numberOfLines = 0;
        context.sizeDelta = new Vector2(0f, textHeight);
    }

    public void Focus()
    {

        Debug.Log("FOCUS");
        ScrollZero();

        if (window.activeSelf)
        {
            command.ActivateInputField();
        }
    }

    private void ScrollZero()
    {
        if (window.activeSelf)
        {
            StartCoroutine(ScrollToZero()); //Hack to force it to go zero sometimes it stays at 0.05f which is considered zero by system
        }
        else
        {
            scrollBar.value = 0f;
        }
    }

    private bool IncrementIndex(int increment)
    {
        if (remainOnIndex >= 1)
        {
            index += increment;
            return true;
        }

        remainOnIndex++;
        return false;

    }

    private bool AutoFill(string[] substrings, IEnumerable<string> values, IEnumerable<string> descriptions = null)
    {
        List<string> descriptionList = null;

        if (descriptions != null)
        {
            descriptionList = descriptions.ToList<string>();
        }

        int index = 0;
        foreach (string val in values)
        {
            index++;
            string args = "";

            if (descriptionList != null && descriptionList[index].Length > 0)
            {
                args = ":";
            }

            if (substrings[substrings.Length - 1].Length > 0 && val.Contains(substrings[substrings.Length - 1]))
            {
                if (val.IndexOf(substrings[substrings.Length - 1][0]) > 0) continue;

                string final = "";

                for (int i = 0; i < substrings.Length - 1; i++)
                {
                    final += substrings[i] + " ";
                }

                //int prevLength = final.Length;

                final += val + args;

                command.text = final;
                command.caretPosition = command.text.Length;
                //command.selectionAnchorPosition = prevLength;
                //command.selectionFocusPosition = command.text.Length;
                return true;
            }
        }

        return false;
    }

    private void DisplayCache()
    {
        if (index < 0)
        {
            index = cacheCommands.Count - 1;
        }
        else if (index >= cacheCommands.Count)
        {
            index = 0;
        }

        if (cacheCommands.Count > 0 && index < cacheCommands.Count)
        {
            Debug.Log("cache[index]=" + cacheCommands[index] + " index=" + index);
            command.text = cacheCommands[index];
        }

        command.caretPosition = command.text.Length;
        //command.selectionAnchorPosition = 0;
        //command.selectionFocusPosition = command.text.Length;
    }
}
