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
    public static ConsoleHandler _init;

    public static ConsoleHandler init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public delegate void OnConsoleEvent(string command);
    public static OnConsoleEvent onConsoleEvent;

    public Text console;
    public InputFieldOverride command;
    public Scrollbar scrollBar;
    public RectTransform context;
    public List<string> cache;
    public int index;
    public int maxNumberOfLines;
    public float textHeight;

    public bool runOnce;

    private int numberOfLines;

    private int remainOnIndex;

    private void Awake()
    {
        _init = this;
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
            else if (onConsoleEvent != null)
            {
                onConsoleEvent(command.text);
            }

            if (!cache.Contains(command.text))
            {
                cache.Add(command.text);
                index++;
            }
            remainOnIndex = 0;

            command.text = "";
        }
        else
        {
            command.DeactivateInputField();
        }
    }

    private bool OverrideGetKey(KeyCode key)
    {
        if (InputOverride.GetKey(key,command.gameObject,true) && !runOnce)
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
        for(int i = 0; i < 3;i++)
        {
            scrollBar.value = 0f;
            yield return null;
        }
    }

    public void AddLine(string line)
    {
        if (++numberOfLines > maxNumberOfLines)
        {
            Clear();
            numberOfLines = 0;
            context.sizeDelta = new Vector2(0f, textHeight);
        }

        console.text += line + "\n";
        context.sizeDelta = new Vector2(0f, context.rect.height + textHeight);
        ScrollZero();
    }

    public void AddLines(string[] lines)
    {
        foreach (string line in lines)
        {
            AddLine(line);
        }
    }

    public void Clear()
    {
        console.text = "";
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
            index+=increment;
            return true;
        }

        remainOnIndex++;
        return false;
        
    }

    private void DisplayCache()
    {
        if (index < 0)
        {
            index = cache.Count - 1;
        }
        else if (index >= cache.Count)
        {
            index = 0;
        }

        if (cache.Count > 0 && index < cache.Count)
        {
            Debug.Log("cache[index]="+ cache[index]+ " index="+index);
            command.text = cache[index];
        }

        command.caretPosition = command.text.Length;
    }
}
