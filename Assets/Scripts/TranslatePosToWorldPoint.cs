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

    public GameObject grid;
    public Camera cm;

    public static Vector3 pos
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

    private static GameObject _grid;
    private static Camera _cm;
    private static Vector3 _pos;

    private void Awake()
    {
        TranslatePosToWorldPoint._grid = grid;
        TranslatePosToWorldPoint._cm = cm;
        _pos = Vector3.zero;
    }

    /*private void Update ()
    {
        //SetPos(Input.mousePosition, out pos);
        //Debug.Log(pos);
	}*/

    public static Vector3 SetPos(Vector3 mousePos, out Vector3 point)
    {
        Vector3 mPos = mousePos;
        mPos.z = _grid.transform.position.z - _cm.transform.position.z;

        point = _cm.ScreenToWorldPoint(mPos);
        point.z = 0f;

        return mPos;
    }
}
