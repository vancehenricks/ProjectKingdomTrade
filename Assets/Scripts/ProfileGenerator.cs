/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ProfileGenerator : MonoBehaviour {

	public List<Color> colors;
	public GameObject baseObject;
	public Image shadeImage;
	public Transform spawner;

	private void Start () {

		foreach(Color color in colors) {
			shadeImage.color = color;
			GameObject obj = Instantiate(baseObject, spawner.position, spawner.rotation, baseObject.transform.parent);
			obj.SetActive(true);

			spawner.position = new Vector3(spawner.position.x,spawner.position.y,0f);
		}
	}
}
