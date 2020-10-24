/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfoGetter : MonoBehaviour {

	public TileInfo tileInfo;

	private void OnTriggerEnter2D (Collider2D col)
    {
		tileInfo = col.gameObject.GetComponent<TileInfo>();
	}
}
