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

    public System.Action<string> onConsoleEvent;
    public GameObject window;
    public InputFieldOverride baseConsole;
    public List<InputFieldOverride> consoles;
    public InputFieldOverride command;
    public Scrollbar scrollBar;
    //public RectTransform context;

    public int maxNumberOfLines;
    //public float textHeight;

    public string previousCommand;

    //+this will remain on scene change
    private static int index;
    private static Dictionary<string, Dictionary<string, string>> commandList;
    private static string cacheInput;
    private static List<string> cacheConsole;
    private static List<string> cacheCommands;
    private static int remainOnIndex;
    //-

    public void Awake()
    {
        init = this;

        if (commandList == null)
        {
            index = 0;

            commandList = new Dictionary<string, Dictionary<string, string>>();
            cacheCommands = new List<string>();
            cacheConsole = new List<string>();

            //AddCache("cancel");
            AddCache("help");
            AddCache("clear");
            //commandList.Add("cancel", new Dictionary<string, string>());
            commandList.Add("help", new Dictionary<string, string>());
            
            Dictionary<string,string> clearSubCommands = new Dictionary<string, string>();
            clearSubCommands.Add("*description", "Clear all logs in the console");
            commandList.Add("clear", clearSubCommands);

            Welcome();
            // initialize();
        }
        else
        {
            command.text = cacheInput;
            AddLines(cacheConsole.ToArray(), false);
            AddLine("Queued commands cancelled!");
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
            CDebug.Log(this, "CONSOLE UP");
            IncrementIndex(1);
            DisplayCache();
        }

        if (OverrideGetKeyUp(KeyCode.DownArrow))
        {
            CDebug.Log(this, "CONSOLE DOWN");
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
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
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
            /*else if (command.text == "cancel")
            {
                foreach (var cmd in commandList)
                {
                    onConsoleEvent(cmd.Key + " cancel");
                }
            }*/
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
        AddLine("");
        AddLine("COMMANDS:");
        foreach (var cmd in commandList)
        {
            if(cmd.Value.ContainsKey("*description"))
            {
                string description = cmd.Value["*description"];
                description = description.PadLeft(description.Length+3);
                AddLine(cmd.Key + description);
            }
            else
            {
                AddLine(cmd.Key);
            }
        }
    }

    public void DisplaySubCommands(string text)
    {
        Dictionary<string, string> subCommands = commandList[text];
        AddLine("");
        AddLine("COMMAND:");
        AddLine($"{text}");
        if(subCommands.ContainsKey("*description"))
        {
            AddLine("DESCRIPTION:");
            AddLine(subCommands["*description"]);
        }
        AddLine("PARAMETERS:");
        foreach (var subCommand in subCommands)
        {
            if(subCommand.Key.Contains("*")) continue;

            if (subCommand.Value != "")
            {
                AddLine($"{subCommand.Key}:{subCommand.Value}");
            }
            else
            {
                AddLine($"{subCommand.Key}");
            }
        }

        if(subCommands.ContainsKey("*examples"))
        {
            AddLine("EXAMPLES:");
            AddLines(subCommands["*examples"].Split('\n'));
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
        if (InputOverride.init.GetKeyUp(key, command.gameObject, true))
        {
            return true;
        }

        return false;
    }

    private bool OverrideGetKeyDown(KeyCode key)
    {
        if (InputOverride.init.GetKeyDown(key, command.gameObject, true))
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
        if (consoles.Count >= maxNumberOfLines)
        {
            Trunked();
        }

        InputFieldOverride console = Instantiate<InputFieldOverride>(baseConsole);
        console.transform.SetParent(baseConsole.transform.parent);
        console.gameObject.SetActive(true);
        console.text += line + Environment.NewLine;
        CDebug.Log(this, $"Command={line}", LogType.Warning);
        consoles.Add(console);

        if (record)
        {
            cacheConsole.Add(line);
        }
        //context.sizeDelta = new Vector2(0f, context.rect.height + textHeight);
        ScrollZero();
    }

    public void AddLines(string[] lines, bool record = true)
    {
        foreach (string line in lines)
        {
            AddLine(line, record);
        }
    }

    private void Welcome()
    {
        AddLine("Version: " + Application.version);
        AddLine("Type \"help\" or press TAB for more info.");
    }

    public void Trunked()
    {
        Destroy(consoles[0].gameObject);
        consoles.RemoveAt(0);
        cacheConsole.RemoveAt(0);
    }

    public void Clear()
    {
        foreach (InputFieldOverride inputField in consoles)
        {
            Destroy(inputField.gameObject);
        }

        consoles.Clear();
        cacheConsole.Clear();
        //context.sizeDelta = new Vector2(0f, textHeight);
        Welcome();
    }

    public void Focus()
    {

        CDebug.Log(this, "CONSOLE FOCUS");
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

        if(substrings[substrings.Length - 1].Contains("*")) return false;

        int index = 0;
        foreach (string val in values)
        {

            string args = "";

            if (descriptionList != null && descriptionList[index].Length > 0)
            {
                args = ":";
            }

            index++;

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
            CDebug.Log(this, "cache[index]=" + cacheCommands[index] + " index=" + index);
            command.text = cacheCommands[index];
        }

        command.caretPosition = command.text.Length;
        //command.selectionAnchorPosition = 0;
        //command.selectionFocusPosition = command.text.Length;
    }
}
