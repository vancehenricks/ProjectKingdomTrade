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

public class SyncSize : MonoBehaviour
{

    public RectTransform basedRect;
    public List<RectTransform> rectList;

    public delegate void DelegateDoSync();
    public static DelegateDoSync doSync;

    private void Awake()
    {
        doSync += DoSync;
    }

    private void DoSync()
    {
        foreach (RectTransform rect in rectList)
        {
            rect.sizeDelta = new Vector2(basedRect.rect.width, basedRect.rect.height);
        }
    }

    private void OnDestroy()
    {
        doSync = null;
    }
}
