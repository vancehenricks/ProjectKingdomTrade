/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, Febuary 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCycler : MonoBehaviour
{

    public TileInfo tileInfo;

    private IEnumerator Cycle(List<UnitInfo> unitInfos)
    {
        yield return new WaitForSeconds(2);

        int visibleIndex = 0;

        while (true)
        {
            //reset:
            for (int i = 0; i < unitInfos.Count; i++)
            {
                /*if (unitInfos[i] == null)
                {
                    unitInfos.RemoveAt(i);
                    goto reset;
                }*/

                if (unitInfos[i] == null) continue;

                GameObject unit = unitInfos[i].tileCaller.image;

                if (visibleIndex != i)
                {
                    unit.SetActive(false);
                }
                else
                {
                    unit.SetActive(true);
                }
            }

            if (visibleIndex + 1 < unitInfos.Count)
            {
                visibleIndex++;
            }
            else
            {
                visibleIndex = 0;
            }

            //Debug.Log("Cycling...");

            yield return new WaitForSeconds(2);
        }
    }

    public void StartCycle(UnitInfo unit)
    {
        StopAllCoroutines();
        tileInfo.unitInfos.Add(unit);

        if (tileInfo.unitInfos.Count <= 1) return;
        StartCoroutine(Cycle(tileInfo.unitInfos));
    }

    public void StopCycle(UnitInfo unit)
    {
        StopAllCoroutines();

        for (int i = 0; i < tileInfo.unitInfos.Count; i++)
        {
            if (tileInfo.unitInfos[i] == null) continue;

            if (tileInfo.unitInfos[i].tileId == unit.tileId)
            {
 
                GameObject visibleUnit = tileInfo.unitInfos[i].tileCaller.image;
                visibleUnit.SetActive(true);
                tileInfo.unitInfos.RemoveAt(i);

                if (tileInfo.unitInfos.Count > 0)
                {
                    GameObject visibleUnit2 = tileInfo.unitInfos[0].tileCaller.image;
                    visibleUnit2.SetActive(true);
                }

                break;
            }
        }

        if (tileInfo.unitInfos.Count > 1)
        {
            StartCoroutine(Cycle(tileInfo.unitInfos));
        }
    }
}
