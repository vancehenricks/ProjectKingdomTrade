/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignTerritory : MonoBehaviour
{
    public TileInfo baseTileInfo;
    public TileEffect tileEffect;

    private void OnTriggerEnter2D(Collider2D col)
    {

        if (col.gameObject.layer != this.gameObject.layer)
        {
            tileEffect = col.gameObject.GetComponent<TileEffect>();

            if (tileEffect != null && tileEffect.tileInfo.tileType != "Edge")
            {
                if (tileEffect.tileInfo.playerInfo == null)
                {
                    tileEffect.tileInfo.playerInfo = baseTileInfo.playerInfo;
                }

                tileEffect.tileInfo.claimants.Add(baseTileInfo.playerInfo);
                baseTileInfo.playerInfo.claims.Add(tileEffect.tileInfo);
                tileEffect.UpdateTerritoryColor();
            }
        }

    }
}
