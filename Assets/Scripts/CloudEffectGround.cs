/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudEffectGround : MonoBehaviour
{

	public TileInfo tileInfo;
	public TileEffect tileEffect;
    public BackgroundInfo backgroundInfo;
    public CloudAction cloudAction;

    private void OnTriggerEnter2D(Collider2D col)
    {

        if (col.gameObject.layer != this.gameObject.layer)
        {
            tileEffect = col.gameObject.GetComponent<TileEffect>();

            if (tileEffect == null)
            {
                return;
            }

            tileInfo = tileEffect.tileInfo;
            tileInfo.localTemp = Temperature.temperature;
            tileEffect.UpdateTileEffect();
        }

        BackgroundInfo info = col.GetComponent<BackgroundInfo>();

        if (info == null) return;

        if (info.type == backgroundInfo.type)
        {
            cloudAction.markedForDestroy = true;
        }
	}

}
