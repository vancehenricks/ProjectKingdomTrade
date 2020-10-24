/* Copyright (C) 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, Febuary 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDirection : MonoBehaviour {

    public PathFinding pathfinding;
    public UnitInfo unitInfo;

    private void Start()
    {
        Tick.tickUpdate += TickUpdate;
        unitInfo.onEnd += OnEnd;
    }

    private void OnEnd()
    {
        Tick.tickUpdate -= TickUpdate;
    }

	private void TickUpdate ()
    {
        if (pathfinding.destination.tile == null) return;
        SetDirection(transform.position, pathfinding.destination.tile.transform.position);
	}

    public void SetDirection(Vector3 currentPosition, Vector3 newPosition)
    {
        Quaternion rotation = unitInfo.tileCaller.image.transform.rotation;
        Transform currentTransform = unitInfo.tileCaller.image.transform;

        //left
        if (newPosition.x == 0) return;

        //Debug.Log(currentPosition.x + ":" + newPosition.x);

        if (currentPosition.x > newPosition.x)
        {
            //Debug.Log("LEFT FACING");
            currentTransform.rotation = new Quaternion(rotation.x, 180, rotation.z, rotation.w);
        }

        //right
        if (currentPosition.x < newPosition.x)
        {
            //Debug.Log("RIGHT FACING");
            currentTransform.rotation = new Quaternion(rotation.x, 0, rotation.z, rotation.w);
        }
    }
}
