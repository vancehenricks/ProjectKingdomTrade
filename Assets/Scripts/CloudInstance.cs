/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, May 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

[System.Serializable]
public struct Cloud
{
    public Vector3 pos;
    public Vector3 posA;
    public Vector3 posB;
    public float offsetDespawn;
    public float liveTime;
    public float liveTimeCounter;
    public float zLevel;
    public float speedModifier;
    public int tickSpeed;
    public float deltaTime;
    public Vector3 newPos;
    public bool outOfBounds;
    public bool markedForDestroy;
    public float posX;
}

public class CloudInstance
{
    public System.Action<Cloud> result;

    private Cloud cloud;

    public CloudInstance(System.Action<Cloud> _result)
    {
        result = _result;
    }

    public void Set(Cloud _cloud)
    {
        cloud = _cloud;
    }

    public void Calculate()
    {
        Cloud newCloud = new Cloud();

        float diffXA = cloud.pos.x - cloud.posA.x;
        float diffXB = cloud.pos.x - cloud.posB.x;

        newCloud.posX = cloud.pos.x + (cloud.tickSpeed * cloud.speedModifier) * cloud.deltaTime;

        newCloud.newPos = new Vector3(cloud.pos.x + (cloud.tickSpeed * cloud.speedModifier) * cloud.deltaTime, cloud.pos.y, cloud.zLevel);
        newCloud.liveTimeCounter = cloud.liveTimeCounter + (cloud.tickSpeed * cloud.deltaTime);

        if (diffXA >= -cloud.offsetDespawn && diffXA <= cloud.offsetDespawn
            || diffXB >= -cloud.offsetDespawn && diffXB <= cloud.offsetDespawn
            || cloud.liveTimeCounter >= cloud.liveTime || cloud.markedForDestroy || cloud.speedModifier == 0f)
        {
            newCloud.outOfBounds = true;
        }

        result(newCloud);
    }
}