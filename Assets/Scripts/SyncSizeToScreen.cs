/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, July 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncSizeToScreen : MonoBehaviour
{

    public RectTransform window;
    public Camera cm;

    private void Start()
    {
        SyncSize.init.doSync += DoSync;
    }

    private void DoSync()
    {
        float distance = Vector3.Distance(window.position, cm.transform.position);
        //Debug.Log("Distance="+distance);
        window.sizeDelta = new Vector2(Screen.width + distance * 2, Screen.height + distance * 2);
    }
}
