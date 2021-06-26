/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, July 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class DragSelect : MonoBehaviour
{
    public GameObject selectorObj;
    public OpenRightClick openRightClick;

    public Vector2 offset;

    public Vector3 p1;
    public Vector3 p2;
    public float zLevel;

    private Vector3 initialMousePos;
    private RectTransform _selectorObjRect;
    private BoxCollider2D _selectorObjCollider;
    private TileInfoGetterArray tileInfoGetterArray;


    // Use this for initialization
    private void Start()
    {
        _selectorObjRect = selectorObj.GetComponent<RectTransform>();
        _selectorObjCollider = selectorObj.GetComponent<BoxCollider2D>();
        tileInfoGetterArray = selectorObj.GetComponent<TileInfoGetterArray>();

        DragHandler.init.overrideOnBeginDrag += OverrideOnBeginDrag;
        DragHandler.init.overrideOnDrag += OverrideOnDrag;
        DragHandler.init.overrideOnEndDrag += OverrideOnEndDrag;
    }

    private void Update()
    {
        //UpdateOffsetPos(InitialmPos, out _offset);
    }

    public void OverrideOnBeginDrag(PointerEventData eventData)
    {
        if (Input.GetButton("Fire1"))
        {
            selectorObj.SetActive(true);
            initialMousePos = TranslatePosToWorldPoint.init.SetPos(Input.mousePosition, out p1);
            selectorObj.transform.position = new Vector3(p1.x, p1.y, zLevel);
            tileInfoGetterArray.Clear();
        }
    }

    public void OverrideOnDrag(PointerEventData eventData)
    {
        if (Input.GetButton("Fire1"))
        {
            //tileInfoGetterArray.holdList = false;

            TranslatePosToWorldPoint.init.SetPos(Input.mousePosition, out p2);
            CDebug.Log(this,"initialMousePos="+initialMousePos+"CurrentMos="+Input.mousePosition);

            if (Input.mousePosition.x > initialMousePos.x && Input.mousePosition.y < initialMousePos.y)
            {
                CDebug.Log(this,"LEFT TOP > RIGHT DOWN");
                _selectorObjRect.pivot = new Vector2(0, 1);
                _selectorObjRect.sizeDelta = new Vector2(p2.x - p1.x, p1.y - p2.y);
                _selectorObjCollider.offset = new Vector2(_selectorObjRect.sizeDelta.x * 0.5f, _selectorObjRect.sizeDelta.y * -0.5f);
            }
            else if (Input.mousePosition.x < initialMousePos.x && Input.mousePosition.y > initialMousePos.y)
            {
                CDebug.Log(this,"RIGHT DOWN > LEFT TOP");
                _selectorObjRect.pivot = new Vector2(1, 0);
                _selectorObjRect.sizeDelta = new Vector2(p1.x - p2.x, p2.y - p1.y);
                _selectorObjCollider.offset = new Vector2(_selectorObjRect.sizeDelta.x * -0.5f, _selectorObjRect.sizeDelta.y * 0.5f);
            }
            else if (Input.mousePosition.x > initialMousePos.x && Input.mousePosition.y > initialMousePos.y)
            {
                CDebug.Log(this,"LEFT DOWN > RIGHT TOP");
                _selectorObjRect.pivot = new Vector2(0, 0);
                _selectorObjRect.sizeDelta = new Vector2(p2.x - p1.x, p2.y - p1.y);
                _selectorObjCollider.offset = new Vector2(_selectorObjRect.sizeDelta.x * 0.5f, _selectorObjRect.sizeDelta.y * 0.5f);
            }
            else
            {
                CDebug.Log(this,"RIGHT TOP > LEFT DOWN");
                _selectorObjRect.pivot = new Vector2(1, 1);
                _selectorObjRect.sizeDelta = new Vector2(p1.x - p2.x, p1.y - p2.y);
                _selectorObjCollider.offset = new Vector2(_selectorObjRect.sizeDelta.x * -0.5f, _selectorObjRect.sizeDelta.y * -0.5f);
            }

            _selectorObjCollider.size = _selectorObjRect.sizeDelta;
        }
    }

    public void OverrideOnEndDrag(PointerEventData eventData)
    {
        //tileInfoGetterArray.holdList = true;
        tileInfoGetterArray.Scan();
        openRightClick.ResetValues();
        openRightClick.forceDisplay = true;

        TileInfoRaycaster.init.tileInfos = tileInfoGetterArray.tileInfos;

        selectorObj.SetActive(false);
        _selectorObjRect.sizeDelta = Vector2.one;
        _selectorObjCollider.size = Vector2.one;
    }
}
