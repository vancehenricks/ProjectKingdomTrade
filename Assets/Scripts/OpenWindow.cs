/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, January 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class OpenWindowInfo
{
    public GameObject window;
    public bool openOnce;
}

public class OpenWindow : MonoBehaviour
{

    public List<OpenWindowInfo> windowList;

    public void DoOpen()
    {

        foreach (OpenWindowInfo windowInfo in windowList)
        {
            CDebug.Log(this, "Opening window=" + windowInfo.window.name, LogType.Log);

            if(windowInfo.openOnce && windowInfo.window.activeSelf) continue;

            windowInfo.window.SetActive(!windowInfo.window.activeSelf);
            windowInfo.window.transform.SetAsLastSibling();
        }
    }

}
