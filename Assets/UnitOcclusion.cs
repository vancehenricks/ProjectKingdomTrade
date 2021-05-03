/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, May 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitOcclusion : MonoBehaviour
{
    private UnitInfo unitInfo;

    private void Start()
    {
        unitInfo = GetComponent<UnitInfo>();
    }

    private void Update()
    {

        if (transform.GetSiblingIndex() == transform.parent.childCount - 1)
        {
            unitInfo.unitEffect.image.SetActive(true);
        }
        else if (transform.parent.name != "Grid")
        {
            unitInfo.unitEffect.image.SetActive(false);
        }
    }
}
