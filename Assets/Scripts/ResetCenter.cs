/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, January 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCenter : MonoBehaviour {

	public Camera cm;

	public KeyCode key;

	private Vector3 originalPos;

	private void Start () {
		originalPos = cm.transform.position;
	}
		
	private void Update () {

		if(InputOverride.GetKeyUp(key))
        {
			DoAction();
		}
	}

	public void DoAction() {
		cm.transform.position = originalPos;
	}
}
