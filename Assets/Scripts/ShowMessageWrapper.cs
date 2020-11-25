/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, November 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShowMessageWrapper : MonoBehaviour
{
    public ShowMessage showMessage;

    public string title;
    public string message;
    public string confirmLabel;
    public string denyLabel;
    public UnityEvent onConfirm;
    public UnityEvent onDeny;

    public void DoAction()
    {
        showMessage.SetMessage(title, message, confirmLabel, denyLabel, null, OnResponse);
    }

    private void OnResponse(bool response)
    {
        if (response)
        {
            onConfirm.Invoke();
        }
        else
        {
            onDeny.Invoke();
        }
    }
}
