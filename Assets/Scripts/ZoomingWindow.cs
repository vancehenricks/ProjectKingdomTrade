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
    public float maxScaleMultipler;
    public float maxScale;
    public float minScale;
    private bool enable;
    private Vector3 oldMousePos;
    private Coroutine delayStop;

    public void Initialize()
    {
        maxScale += (maxScale * maxScaleMultipler);
        enable = true;
    }

    private void OnDestroy()
    {
        if(delayStop != null)
        {
            StopCoroutine(delayStop);
        }
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (!enable) return;

        if (grid.rect.width > defaultWidth)
        {
            speed = (defaultSpeed + (grid.rect.width / divideWidth));
        }

        if(delayStop != null)
        {
            StopCoroutine(delayStop);
        }

        Vector3 preZoom = TranslatePosToWorldPoint.init.pos;
        scale = cm.transform.position.z;

        if (InputOverride.init.GetAxis("Mouse ScrollWheel") > 0 && scale <= maxScale)
        {
            scale += speed;
        }

        if (InputOverride.init.GetAxis("Mouse ScrollWheel") < 0 && scale >= minScale)
        {
            scale -= speed;
        }

        cm.transform.position = new Vector3(preZoom.x, preZoom.y, preZoom.z+scale);

        Vector3 postZoom = TranslatePosToWorldPoint.init.pos;
        Vector3 diffZoom = preZoom - postZoom;

        cm.transform.position = new Vector3(preZoom.x+diffZoom.x, preZoom.y+diffZoom.y, preZoom.z+scale);

        if(CursorReplace.init.currentCursor != CursorType.Zoom)
        {
            CursorReplace.init.SetCurrentCursorAsPrevious();
        }

        CursorReplace.init.currentCursor = CursorType.Zoom;

        oldMousePos = Input.mousePosition;
        delayStop = StartCoroutine(DelayStop());
    }

    private IEnumerator DelayStop()
    {
        yield return new WaitForSeconds(1f);

        while(Input.mousePosition == oldMousePos)
        {
            yield return null;
        }

        CursorReplace.init.currentCursor = CursorType.Previous;
    }

}
