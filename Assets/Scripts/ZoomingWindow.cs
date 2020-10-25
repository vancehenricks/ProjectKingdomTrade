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
    public bool isScrolling;
    public float threshold;
    public Vector2 offset;
    public Vector3 position;
    //public float zoomThreshold;
    //public float zoomCounter;
    //public float distance;

    private void Update()
    {
        if (!isScrolling)
        {
            position = TranslatePosToWorldPoint.pos;
        }
        else if (Vector3.Distance(position, TranslatePosToWorldPoint.pos) > threshold)
        {
            isScrolling = false;
            CursorReplace.currentCursor = CursorType.Previous;
            //zoomCounter = 0;
        }
    }

    public void OnScroll(PointerEventData eventData)
    {
        //if (zoomCounter++ <= zoomThreshold) return;

        if (grid.rect.width > defaultWidth)
        {
            speed = (defaultSpeed + (grid.rect.width / divideWidth));
        }

        scale = cm.transform.position.z;

        if (InputOverride.GetAxis("Mouse ScrollWheel") > 0 && scale <= maxScale)
        {
            isScrolling = true;
            scale = cm.transform.position.z + speed;
        }

        if (InputOverride.GetAxis("Mouse ScrollWheel") < 0 && scale >= minScale)
        {
            isScrolling = true;
            scale = cm.transform.position.z - speed;
        }

        //UnityEditor only isssue where escape button looses focus
        if (isScrolling)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.lockState = CursorLockMode.None;
            CursorReplace.currentCursor = CursorType.Zoom;
            cm.transform.position = new Vector3(position.x + offset.x, position.y + offset.y, scale);
        }
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.lockState = CursorLockMode.None;

        //StartCoroutine(DelayStop());
    }

}
