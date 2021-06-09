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


public class OpenWindow : MonoBehaviour
{

    public List<GameObject> windowList;

    public void DoOpen()
    {

        foreach (GameObject window in windowList)
        {
            CDebug.Log(this, "Opening window=" + window.name, LogType.Log);

            window.SetActive(!window.activeSelf);
            window.transform.SetAsLastSibling();
        }
    }

}
