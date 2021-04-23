/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, August 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslatePosToWorldPoint : MonoBehaviour
{
    private static TranslatePosToWorldPoint _init;

    public static TranslatePosToWorldPoint init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public GameObject grid;
    public Camera cm;

    public Vector3 pos
    {
        get
        {
            SetPos(Input.mousePosition, out _pos);
            return _pos;
        }
        private set
        {
            _pos = value;
        }
    }

    private Vector3 _pos;

    private void Awake()
    {
        init = this;
    }


    public Vector3 SetPos(Vector3 mousePos, out Vector3 point)
    {
        Vector3 mPos = mousePos;
        mPos.z = grid.transform.position.z - cm.transform.position.z;

        point = cm.ScreenToWorldPoint(mPos);
        point.z = 0f;

        return mPos;
    }
}
