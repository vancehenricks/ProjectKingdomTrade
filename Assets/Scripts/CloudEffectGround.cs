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

            tileEffect.UpdateTileEffect(cloudAction, false);

        }

        CloudAction cloud = col.GetComponent<CloudAction>();

        if (cloud == null) return;

        if (tileEffect != null && tileEffect.tileInfo.tileType != "Edge" && cloudAction.type == "Cloud" && cloud.type == "Cloud")
        {
            CloudCycle.init.GenerateTornado(cloudAction, cloud);
        }
        else if (cloudAction.type == "Cloud")
        {
            cloudAction.markedForDestroy = true;
        }
        else if (cloudAction.type == "Tornado" && cloud.type == "Cloud")
        {
            cloudAction.liveTimeCounter -= cloud.collidePoints;
        }
        else if (cloudAction.type == "Tornado" && cloud.type == "Tornado")
        {
            CloudCycle.init.GenerateTornado(cloudAction, cloud);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer != this.gameObject.layer)
        {
            tileEffect = col.gameObject.GetComponent<TileEffect>();

            if (tileEffect == null)
            {
                return;
            }

            tileEffect.UpdateTileEffect(cloudAction, true);
        }
    }

}
