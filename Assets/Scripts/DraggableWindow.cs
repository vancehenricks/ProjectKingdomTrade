/* Copyright (C) 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, January 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableWindow : MonoBehaviour, IBeginDragHandler, IDragHandler
{

	public Vector2 offSet;
	public GameObject window;

	private Vector2 normalizedPos;

	private void Start ()
    {
		if(window == null)
        {
			window = this.gameObject;
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
    {
        offSet = new Vector2(window.transform.position.x, window.transform.position.y) - eventData.position;
	}
		
	public void OnDrag(PointerEventData eventData)
    {
        normalizedPos = eventData.position + offSet;

		Debug.Log("Offset_Pos:" + offSet + " rMouse_Pos:" + eventData.position + " nMouse_Pos:" + normalizedPos);
		window.transform.position = normalizedPos;
	}
}
