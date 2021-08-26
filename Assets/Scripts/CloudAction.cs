/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, January 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

public struct CloudActionValues
{
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

public class CloudAction : BaseInfo
{
    public Camera cm;
    //public string type;
    public float spawnChance;
    public CloudCycle cloudCycle;
    public RectTransform grid;
    public float divideWidth;
    public float defaultWidth;
    public bool influenceByGrid;
    public float minLiveTime;
    public float maxLiveTime;
    public float speedModifier;
    public Transform posA;
    public Transform posB;
    public float offsetDespawn;

    public float maxAlpha;
    public float durationBeforeDisplay;
    public bool markedForDestroy;

    public float collidePoints;
    public float liveTimeCounter;
    private float liveTime;
    public Image image;
    public bool visible;
    public int tickCountMax;

    private CloudActionValues cloud;
    private ParallelInstance<CloudActionValues> parallellInstance;

    private Coroutine move;

    private void Start()
    {
        base.Initialize();

        if (grid.rect.width > defaultWidth && influenceByGrid)
        {
            maxLiveTime *= (grid.rect.width / divideWidth);
        }

        liveTime = Random.Range(minLiveTime, maxLiveTime);

        cloud = new CloudActionValues();
        parallellInstance = new ParallelInstance<CloudActionValues>(Calculate, (CloudActionValues newCloud) => {cloud = newCloud;});

        StartCoroutine(FadeIn(0.1f));
        move = StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        while (true)
        {
            if (Tick.init.speed > 0)
            {
                cloud.deltaTime = Time.deltaTime;
                cloud.liveTime = liveTime;
                cloud.speedModifier = speedModifier;
                cloud.tickSpeed = Tick.init.speed;
                cloud.pos = new Vector3(transform.position.x, transform.position.y, cloudCycle.zLevel);
                cloud.offsetDespawn = offsetDespawn;
                cloud.posA = posA.transform.position;
                cloud.posB = posB.transform.position;
                cloud.occlusion = new OcclusionValue(cm.WorldToScreenPoint(transform.position), new Vector2Int(cm.pixelWidth, cm.pixelHeight),
                 TileOcclusion.init.overflow);

                Task task = parallellInstance.Start(cloud);
                
                while(!task.IsCompleted)
                {
                    yield return null;
                }  

                liveTimeCounter = cloud.liveTimeCounter;
                Tools.SetDirection(transform, cloud.newPos);
                gameObject.transform.position = cloud.newPos;

                if (cloud.enabledImage == true && visible == true)
                {
                    image.enabled = true;
                }
                else
                {
                    image.enabled = false;
                }

                if (cloud.outOfBounds || markedForDestroy)
                {
                    StartCoroutine(FadeOut(0.1f));
                }
            }

            yield return null;
        }
    }

    //Seperate thread+
    private void Calculate(System.Action<CloudActionValues> result, CloudActionValues _cloud)
    {
        CloudActionValues newCloud = _cloud;

        float diffXA = newCloud.pos.x - cloud.posA.x;
        float diffXB = newCloud.pos.x - cloud.posB.x;

        newCloud.newPos = Vector3.MoveTowards(newCloud.pos, new Vector3( newCloud.posB.x, newCloud.pos.y, newCloud.pos.z), 
            (newCloud.tickSpeed * newCloud.speedModifier) * newCloud.deltaTime);

        newCloud.liveTimeCounter = newCloud.liveTimeCounter + (newCloud.tickSpeed * newCloud.deltaTime);

        if (diffXA >= -newCloud.offsetDespawn && diffXA <= newCloud.offsetDespawn
            || diffXB >= -newCloud.offsetDespawn && diffXB <= newCloud.offsetDespawn
            || newCloud.liveTimeCounter >= newCloud.liveTime || newCloud.speedModifier == 0f)
        {
            newCloud.outOfBounds = true;
        }

        newCloud.enabledImage = Tools.IsWithinCameraView(newCloud.occlusion);

        result(newCloud);
    }
    //Seperate thread-

    private IEnumerator FadeIn(float value)
    {
        Color color = image.color;

        for (int i = 0; i < durationBeforeDisplay; i++)
        {
            if (Tick.init.speed == 0)
            {
                i--;
            }

            yield return new WaitForSeconds(1f);
        }

        while (color.a < maxAlpha)
        {
            color.a += value;
            image.color = color;

            yield return null;
        }
    }

    private void OnDestroy()
    {
        if (move != null)
        {
            StopCoroutine(move);
        }
        cloudCycle.clouds.Remove(this);
        cloudCycle.counter--;
    }

    private IEnumerator FadeOut(float value)
    {
        Color color = image.color;

        while (color.a > 0)
        {
            color.a -= value;
            image.color = color;

            yield return null;
        }

        Destroy(gameObject);
    }
}
