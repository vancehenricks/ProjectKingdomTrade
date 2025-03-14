﻿/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2019
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HideObject : MonoBehaviour
{
    public string identifier;
    public GameObject window;
    public GameObject priorityWindow;
    public KeyCode key;
    public UnityEvent onClose;
    public UnityEvent onOpen;

    private void Start()
    {
        HideObjectHandler.init.hideObjects.Add(identifier, this);
    }

    // Update is called once per frame
    private void Update()
    {
        if (priorityWindow != null)
        {
            if (InputOverride.init.GetKeyUp(key) && !priorityWindow.activeSelf)
            {
                DoAction();
            }
        }
        else
        {
            if (InputOverride.init.GetKeyUp(key))
            {
                DoAction();
            }
        }
    }

    private void DoAction()
    {
        window.SetActive(!window.activeSelf);

        CDebug.Log(this,"WINDOW=" + window.activeSelf,LogType.Warning);

        if (window.activeSelf)
        {
            onOpen.Invoke();
        }
        else
        {
            onClose.Invoke();
        }
    }
}
