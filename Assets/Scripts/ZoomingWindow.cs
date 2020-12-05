﻿/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, January 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ZoomingWindow : MonoBehaviour, IScrollHandler
{

    public Camera cm;
    public RectTransform grid;
    public float divideWidth;
    public float defaultWidth;
    public float defaultSpeed;
    public float speed;
    public float scale;
    public float maxScale;
    public float minScale;

    public void OnScroll(PointerEventData eventData)
    {
        if (grid.rect.width > defaultWidth)
        {
            speed = (defaultSpeed + (grid.rect.width / divideWidth));
        }

        Vector3 preZoom = TranslatePosToWorldPoint.pos;
        scale = cm.transform.position.z;

        if (InputOverride.GetAxis("Mouse ScrollWheel") > 0 && scale <= maxScale)
        {
            scale += speed;
        }

        if (InputOverride.GetAxis("Mouse ScrollWheel") < 0 && scale >= minScale)
        {
            scale -= speed;
        }

        cm.transform.position = new Vector3(preZoom.x, preZoom.y, preZoom.z+scale);

        Vector3 postZoom = TranslatePosToWorldPoint.pos;
        Vector3 diffZoom = preZoom - postZoom;

        cm.transform.position = new Vector3(preZoom.x+diffZoom.x, preZoom.y+diffZoom.y, preZoom.z+scale);

        CursorReplace.currentCursor = CursorType.Zoom;

        StopAllCoroutines();
        StartCoroutine(DelayStop());
    }

    IEnumerator DelayStop()
    {
        yield return new WaitForSeconds(0.5f);
        CursorReplace.currentCursor = CursorType.Previous;
    }

}
