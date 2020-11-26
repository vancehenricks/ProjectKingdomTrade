/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, March 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ConsoleHandler : MonoBehaviour
{
    private static ConsoleHandler _init;

    public static ConsoleHandler init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public delegate void Initialize();
    public static Initialize initialize;

    public delegate void OnConsoleEvent(string command);
    public static OnConsoleEvent onConsoleEvent;

    public Text console;

    public InputFieldOverride command;
    public Scrollbar scrollBar;
    public RectTransform context;

    public int maxNumberOfLines;
    public float textHeight;

    private bool runOnce;
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
        if (cacheCommands == null)
        {
            index = 0;

            commandList = new Dictionary<string, Dictionary<string, string>>();
            cacheCommands = new List<string>();
            cacheConsole = new List<string>();
            AddCache("cancel");
            AddCache("help");
            AddCache("clear");
            init = this;
            initialize();
        }
        else
        {
            command.text = cacheInput;
            AddLines(cacheConsole.ToArray(), false);
            init = this;
        }
    }

    private void OnDestroy()
    {
        cacheInput = command.text;
        initialize = null;
        onConsoleEvent = null;
    }

    private void Update()
    {
        if (runOnce) return;

        if (OverrideGetKey(KeyCode.UpArrow))
        {
            Debug.Log("UP");
            IncrementIndex(1);
            DisplayCache();
        }

        if (OverrideGetKey(KeyCode.DownArrow))
        {
            Debug.Log("DOWN");
            IncrementIndex(-1);
            DisplayCache();
        }

        if (OverrideGetKey(KeyCode.Tab))
        {
            string[] substrings = command.text.Split(' ');

            if (commandList.ContainsKey(substrings[0]))
            {
                DisplaySubCommands(substrings[0]);
            }
            else
            {
                DisplayCommands();
            }
        }
    }

    public void NextLine()
    {
        if (Input.GetKey(KeyCode.Return))
        {
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

            previousCommand = command.text;

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
        AddLine("\t" + "cancel");
        AddLine("\t" + "help");
        AddLine("\t" + "clear");
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
            index++;
        }
    }

    public void AddCommand(string cmd, Dictionary<string, string> subCmds)
    {
        if (!commandList.ContainsKey(cmd))
        {
            commandList.Add(cmd, subCmds);
        }
    }

    private bool OverrideGetKey(KeyCode key)
    {
        if (InputOverride.GetKey(key, command.gameObject, true) && !runOnce)
        {
            runOnce = true;
            StartCoroutine(Reset());
            return true;
        }

        return false;
    }

    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(0.1f);
        runOnce = false;
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

        console.text += line + "\n";
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

        if (gameObject.activeSelf)
        {
            command.ActivateInputField();
        }
    }

    private void ScrollZero()
    {
        if (gameObject.activeSelf)
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
    }
}
