/* Copyright (C) 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowButtonAction : MonoBehaviour {

	public RectTransform content;
	public float direction;
	public float duration;

	public void DoAction() {
		StartCoroutine(DoActionLogic());
	}

	IEnumerator DoActionLogic() {
		for(int i = 0; i < duration;i++) {
			content.position = new Vector2(content.position.x+direction, content.position.y);
			yield return null;
		}
	}
}
