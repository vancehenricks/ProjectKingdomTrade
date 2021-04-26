/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, March 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

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
        //CDebug.Log(this, currentFocus.GetInstanceID());

        if (CheckFocus(focus, doNothingIfNull))
        {
            return Input.GetKey(key);
        }

        return false;
    }

    public bool GetKeyUp(KeyCode key, GameObject focus = null, bool doNothingIfNull = false)
    {
        //CDebug.Log(this, currentFocus.GetInstanceID());

        if (CheckFocus(focus, doNothingIfNull))
        {
            return Input.GetKeyUp(key);
        }

        return false;
    }

    public bool GetKeyDown(KeyCode key, GameObject focus = null, bool doNothingIfNull = false)
    {
        //CDebug.Log(this, currentFocus.GetInstanceID());

        if (CheckFocus(focus, doNothingIfNull))
        {
            return Input.GetKeyDown(key);
        }

        return false;
    }

    public float GetAxis(string axis, GameObject focus = null, bool doNothingIfNull = false)
    {
        //CDebug.Log(this, currentFocus.GetInstanceID());

        if (CheckFocus(focus, doNothingIfNull))
        {
            return Input.GetAxis(axis);
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
