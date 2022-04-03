/* Copyright 2022 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, April 2022
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

public struct CloudActionValues
{
    public long tileId;
    public Vector3 pos;
    public Vector3 posA;
    public Vector3 posB;
    public float offsetDespawn;
    public float liveTime;
    public float liveTimeCounter;
    public float speedModifier;
    public int tickSpeed;
    public float deltaTime;
    public Vector3 newPos;
    public bool outOfBounds;
    public bool enabledImage;
    public OcclusionValue occlusion;
}

public class CloudActionHandler : MonoBehaviour, IParallelContract<CloudActionValues, CloudAction>
{
    private static CloudActionHandler _init;
    public static CloudActionHandler init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public Camera cm;
    private List<CloudActionValues> result;
    private ParallelInstance<List<CloudActionValues>> parallelInstance;

    private Coroutine sync;

    private void Awake()
    {
        init = this;
    }

    public void Initialize()
    {
        parallelInstance = new ParallelInstance<List<CloudActionValues>>(Calculate,(
            List<CloudActionValues> _result) => 
            {
                result =_result;
            }
        );

       sync = StartCoroutine(Sync());
    }

    private void OnDestroy()
    {
        if(sync != null)
        {
            StopCoroutine(sync);
        }
    }

    public List<CloudActionValues> Convert(List<CloudAction> list)
    {
        List<CloudActionValues> convertList = new List<CloudActionValues>();

        foreach(var cloudAction in list)
        {
            CloudActionValues cloud = new CloudActionValues()
            {
                tileId = cloudAction.tileId,
                deltaTime = Time.deltaTime,
                liveTime = cloudAction.liveTime,
                speedModifier = cloudAction.speedModifier,
                tickSpeed = Tick.init.speed,
                pos = new Vector3(cloudAction.transform.position.x, cloudAction.transform.position.y, CloudCycle.init.zLevel),
                offsetDespawn = cloudAction.offsetDespawn,
                posA = cloudAction.posA.transform.position,
                posB = cloudAction.posB.transform.position,
                occlusion = new OcclusionValue(cm.WorldToScreenPoint(cloudAction.transform.position), 
                new Vector2Int(cm.pixelWidth, cm.pixelHeight),
                    TileOcclusion.init.overflow),
            };
            convertList.Add(cloud);
        }

        return convertList;
    }

    public void Calculate(Action<List<CloudActionValues>> result, List<CloudActionValues> list)
    {
        List<CloudActionValues> newList = new List<CloudActionValues>();

        for(int i = 0;i < list.Count;i++)
        {
            CloudActionValues newCloud = list[i];

            float diffXA = newCloud.pos.x - list[i].posA.x;
            float diffXB = newCloud.pos.x - list[i].posB.x;

            newCloud.newPos = Vector3.MoveTowards(newCloud.pos, new Vector3(newCloud.posB.x, newCloud.pos.y, newCloud.pos.z), 
                (newCloud.tickSpeed * newCloud.speedModifier) * newCloud.deltaTime);

            newCloud.liveTimeCounter = newCloud.liveTimeCounter + (newCloud.tickSpeed * newCloud.deltaTime);

            if (diffXA >= -newCloud.offsetDespawn && diffXA <= newCloud.offsetDespawn
                || diffXB >= -newCloud.offsetDespawn && diffXB <= newCloud.offsetDespawn
                || newCloud.liveTimeCounter >= newCloud.liveTime || newCloud.speedModifier == 0f)
            {
                newCloud.outOfBounds = true;
            }

            newCloud.enabledImage = Tools.IsWithinCameraView(newCloud.occlusion);
            newList.Add(newCloud);
        }

        result(newList);
    }

    public IEnumerator Sync()
    {
        while (true)
        {
            if (Tick.init.speed > 0)
            {
                Task task = parallelInstance.Start(Convert(CloudCycle.init.clouds.Values.ToList()));
                
                while(!task.IsCompleted)
                {
                    yield return null;
                }

                foreach (CloudActionValues cloudActionValues in result)
                {

                    CloudAction cloud;
                    CloudCycle.init.clouds.TryGetValue(cloudActionValues.tileId, out cloud);
                    
                    if(cloud == null) continue;
                    cloud.Move(cloudActionValues);
                }
            }

            yield return null;
        }
    }
}