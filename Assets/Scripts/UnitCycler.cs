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
        yield return new WaitForSeconds(2f);

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
                    unitInfos[i].transform.SetAsLastSibling();
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

        /*for (int i = 0; i < tileInfo.unitInfos.Count; i++)
        {
            if (tileInfo.unitInfos[i] == null) continue;

            if (tileInfo.unitInfos[i].tileId == unit.tileId)
            {

               tileInfo.unitInfos[i].transform.SetAsLastSibling();

                if (tileInfo.unitInfos.Count > 0)
                {
                    tileInfo.unitInfos[0].transform.SetAsLastSibling();
                }

                break;
            }
        }*/

        if (tileInfo.unitInfos.Count > 1)
        {
            StartCoroutine(Cycle(tileInfo.unitInfos));
        }
    }
}
