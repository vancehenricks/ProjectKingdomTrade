/* Copyright (C) 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, March 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideBorder : MonoBehaviour {

    public List<TileInfo> tileExceptions;
	public KeyCode key;

	// Update is called once per frame
	private void Update ()
    {

		if(InputOverride.GetKeyUp(key))
        {

			foreach(GameObject tile in MapGenerator.init.generatedTile.Values)
            {
				TileCaller tileCaller = tile.GetComponent<TileCaller>();
                TileInfo tileInfo = tile.GetComponent<TileInfo>();

                if (tileCaller.shade != null) {

                    bool isException = false;

                    foreach (var tileException in tileExceptions)
                    {
                        if (tileInfo.tileType == tileException.tileType)
                        {
                            isException = true;
                            break;
                        }
                    }

                    if (!isException)
                    {
                        tileCaller.shade.SetActive(!tileCaller.shade.activeSelf);
                    }
                }
			}
		}

	}
}
