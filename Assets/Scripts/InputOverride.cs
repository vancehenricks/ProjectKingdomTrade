/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, March 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DebugHandler;

public class InputOverride : MonoBehaviour
{
    private static InputOverride _init;
    public static InputOverride init
    {
        get { return _init; }
        private set { _init = value; }
    }

    private void Awake()
    {
        init = this;
    }

    private GameObject _currentFocus;

    public GameObject currentFocus
    {
        get
        {

            if (_currentFocus != null && _currentFocus.activeInHierarchy)
            {
                return _currentFocus;
            }
            else
            {
                _currentFocus = null;
                return null;
            }
        }

        set
        {
            if (value != null && value.activeInHierarchy)
            {
                _currentFocus = value;
            }
            else
            {
                _currentFocus = null;
            }
        }
    }


    public bool GetKey(KeyCode key, GameObject focus = null, bool doNothingIfNull = false)
    {
        if (CheckFocus(focus, doNothingIfNull) && Input.GetKey(key))
        {
            CDebug.Log(this, "GetKey=" + key);
            return true;
        }

        return false;
    }

    public bool GetKeyUp(KeyCode key, GameObject focus = null, bool doNothingIfNull = false)
    {
        if (CheckFocus(focus, doNothingIfNull) && Input.GetKeyUp(key))
        {
            CDebug.Log(this, "GetKeyUp=" + key);
            return true;
        }

        return false;
    }

    public bool GetKeyDown(KeyCode key, GameObject focus = null, bool doNothingIfNull = false)
    {
        if (CheckFocus(focus, doNothingIfNull) && Input.GetKeyDown(key))
        {
            CDebug.Log(this, "GetKeyDown=" + key);
            return true;
        }

        return false;
    }

    public float GetAxis(string axis, GameObject focus = null, bool doNothingIfNull = false)
    {
        if (CheckFocus(focus, doNothingIfNull))
        {
            float result = Input.GetAxis(axis);

            if (result > 0f || result < 0f)
            {
                CDebug.Log(this, "GetAxis=" + axis + " result=" + (result > 0f ? "Up" : "Down"));
            }

            return result;
        }

        return 0f;
    }

    private bool CheckFocus(GameObject focus, bool doNothingIfNull)
    {
        if ((!doNothingIfNull && currentFocus == null) ||
            currentFocus != null && focus != null && currentFocus.GetInstanceID() == focus.GetInstanceID())
        {
            return true;
        }

        return false;
    }
}
