/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, March 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideBorder : MonoBehaviour
{

    public List<TileInfo> tileExceptions;
    public KeyCode key;

    // Update is called once per frame
    private void Update()
    {

        if (InputOverride.init.GetKeyUp(key))
        {

            foreach (TileInfo tileInfo in TileList.init.generatedTiles.Values)
            {

                if (tileInfo.tileEffect.shadeImage != null)
                {

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
                        Image image = tileInfo.tileEffect.shadeImage;
                        image.enabled = !image.enabled;
                    }
                }
            }
        }

    }
}
