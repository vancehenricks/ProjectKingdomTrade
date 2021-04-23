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
    private static DragHandler _init;

    public static DragHandler init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public delegate void OverrideOnBeginDrag(PointerEventData eventData);
    public delegate void OverrideOnDrag(PointerEventData eventData);
    public delegate void OverrideOnEndDrag(PointerEventData eventData);

    public OverrideOnBeginDrag overrideOnBeginDrag;
    public OverrideOnDrag overrideOnDrag;
    public OverrideOnEndDrag overrideOnEndDrag;

    private bool onBeginDrag, onDrag, onEndDrag;
    private PointerEventData pOnBeginDrag, pOnDrag, pOnEndDrag;

    private void Awake()
    {
        init = this;
    }

    private void Start()
    {
        CommandPipeline.init.Add(Command, 999);
    }

    private void OnDestroy()
    {
        overrideOnBeginDrag = null;
        overrideOnDrag = null;
        overrideOnEndDrag = null;
    }

    private void Command()
    {
        if (onBeginDrag)
        {
            onBeginDrag = false;
            overrideOnBeginDrag(pOnBeginDrag);
        }

        if (onDrag)
        {
            onDrag = false;
            overrideOnDrag(pOnDrag);
        }

        if (onEndDrag)
        {
            onEndDrag = false;
            overrideOnEndDrag(pOnEndDrag);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        onBeginDrag = true;
        pOnBeginDrag = eventData;
    }

    public void OnDrag(PointerEventData eventData)
    {
        onDrag = true;
        pOnDrag = eventData;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        onEndDrag = true;
        pOnEndDrag = eventData;
    }
}
