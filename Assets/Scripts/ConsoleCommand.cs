/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, November 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConsoleCommand : MonoBehaviour
{
    private void Start()
    {
        ConsoleParser.onParsedConsoleEvent += OnParsedConsoleEvent;
        Initialize();
    }

    private void OnDestroy()
    {
        //if console object is not active at all it will not call OnDestroy on those thus not clearing the delegates
        ConsoleParser.onParsedConsoleEvent -= OnParsedConsoleEvent;
    }

    public abstract void Initialize();
    public abstract void OnParsedConsoleEvent(string command, string[] arguments);
}