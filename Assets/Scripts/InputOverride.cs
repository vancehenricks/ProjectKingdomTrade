/* Copyright (C) 2020 by Vance Henricks Patual - All Rights Reserved
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
    private static GameObject _currentFocus;

    public static GameObject currentFocus
    {
        get
        {
            return _currentFocus;
        }

        set
        {
            if (value != null && value.activeSelf)
            {
                _currentFocus = value;
            }
            else
            {
                _currentFocus = null;
            }
        }
    }


    public static bool GetKey(KeyCode key, GameObject focus = null, bool doNothingIfNull = false)
    {
        //Debug.Log(currentFocus.GetInstanceID());

        if (CheckFocus(focus, doNothingIfNull))
        {
            return Input.GetKey(key);
        }

        return false;
    }

    public static bool GetKeyUp(KeyCode key, GameObject focus = null, bool doNothingIfNull = false)
    {

        if (CheckFocus(focus, doNothingIfNull))
        {
            return Input.GetKeyUp(key);
        }

        return false;
    }

    public static bool GetKeyDown(KeyCode key, GameObject focus = null, bool doNothingIfNull = false)
    {
        if (CheckFocus(focus, doNothingIfNull))
        {
            return Input.GetKeyDown(key);
        }

        return false;
    }

    public static float GetAxis(string axis, GameObject focus = null, bool doNothingIfNull = false)
    {
        if (CheckFocus(focus,doNothingIfNull))
        {
            return Input.GetAxis(axis);
        }

        return 0f;
    }

    private static bool CheckFocus(GameObject focus, bool doNothingIfNull)
    {
        if ((!doNothingIfNull && currentFocus == null) || 
            currentFocus != null && focus != null && currentFocus.GetInstanceID() == focus.GetInstanceID())
        {
            return true;
        }

        return false;
    }
}
