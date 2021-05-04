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
    private TileInfo tileInfo;
    private bool stopCoroutine;
    private bool startCoroutine;
    private Transform previous;


    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void Awake()
    {
        tileInfo = GetComponent<TileInfo>();
        previous = transform;
    }

    private void FixedUpdate()
    {
        if (!stopCoroutine && transform.parent.childCount <= 1)
        {
            stopCoroutine = true;
            startCoroutine = false;
            StopAllCoroutines();
        }
        else if(!startCoroutine && transform.parent.childCount > 1)
        {
            stopCoroutine = false;
            startCoroutine = true;
            StartCoroutine(Cycle());
        }
    }

    private IEnumerator Cycle()
    {
        while (true)
        {
            for (int i = 0; i < transform.parent.childCount; i++)
            {

                tileInfo.transform.SetAsLastSibling();
                yield return new WaitForSeconds(0.3f);

                if (transform.parent.childCount > i && previous == transform.parent.GetChild(i) && transform.parent.childCount > 2)
                {
                    i++;
                }

                if (transform.parent.childCount > i && transform.parent.GetChild(i) != tileInfo.transform)
                {
                    previous = transform.parent.GetChild(i);
                    previous.SetAsLastSibling();

                    yield return new WaitForSeconds(0.5f);
                }
            }
        }
    }
}
