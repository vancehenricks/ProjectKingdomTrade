/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, July 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//Purpose for this is to no longer need graphics raycaster on different gameobject outside of canvas
//https://stackoverflow.com/questions/43457145/unity-onbegindrag-ondrag-e-t-c-not-working

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public delegate void OverrideOnBeginDrag(PointerEventData eventData);
    public delegate void OverrideOnDrag(PointerEventData eventData);
    public delegate void OverrideOnEndDrag(PointerEventData eventData);

    public static OverrideOnBeginDrag overrideOnBeginDrag;
    public static OverrideOnDrag overrideOnDrag;
    public static OverrideOnEndDrag overrideOnEndDrag;

    private void OnDestroy()
    {
        overrideOnBeginDrag = null;
        overrideOnDrag = null;
        overrideOnEndDrag = null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        overrideOnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        overrideOnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        overrideOnEndDrag(eventData);
    }
}
