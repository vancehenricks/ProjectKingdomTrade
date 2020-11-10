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
    public Vector3 position;


    public void OnScroll(PointerEventData eventData)
    {
        position = cm.transform.position;

        if (grid.rect.width > defaultWidth)
        {
            speed = (defaultSpeed + (grid.rect.width / divideWidth));
        }

        scale = cm.transform.position.z;

        if (InputOverride.GetAxis("Mouse ScrollWheel") > 0 && scale <= maxScale)
        {
            scale = cm.transform.position.z + speed;
        }

        if (InputOverride.GetAxis("Mouse ScrollWheel") < 0 && scale >= minScale)
        {
            scale = cm.transform.position.z - speed;
        }

        CursorReplace.currentCursor = CursorType.Zoom;
        cm.transform.position = new Vector3(position.x, position.y, scale);

        StopAllCoroutines();
        StartCoroutine(DelayStop());
    }

    IEnumerator DelayStop()
    {
        yield return new WaitForSeconds(1f);
        CursorReplace.currentCursor = CursorType.Previous;
    }

}
