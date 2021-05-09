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
    public Vector3 direction;
    public bool enabledImage;
    public OcclusionValue occlusion;
}

public class CloudAction : MonoBehaviour
{
    public Camera cm;
    public string type;
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

    private Cloud cloud;
    private ParallelInstance<Cloud> parallellInstance;

    private Coroutine move;

    private void Start()
    {

        if (grid.rect.width > defaultWidth && influenceByGrid)
        {
            maxLiveTime *= (grid.rect.width / divideWidth);
        }

        liveTime = Random.Range(minLiveTime, maxLiveTime);

        cloud = new Cloud();
        cloud.occlusion.overflow = TileOcclusion.init.overflow;

        parallellInstance = new ParallelInstance<Cloud>(Calculate, (Cloud _cloud, Cloud original) => {cloud = _cloud;});

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
                cloud.markedForDestroy = markedForDestroy;
                cloud.tickSpeed = Tick.init.speed;
                cloud.zLevel = cloudCycle.zLevel;
                cloud.pos = transform.position;
                cloud.offsetDespawn = offsetDespawn;
                cloud.posA = posA.transform.position;
                cloud.posB = posB.transform.position;
                cloud.occlusion.screenPos = cm.WorldToScreenPoint(transform.position);
                cloud.occlusion.screenSize = new Vector2Int(cm.pixelWidth, cm.pixelHeight);

                parallellInstance.Set(cloud);
                Task task = new Task(parallellInstance.Calculate);
                task.Start();

                yield return null;
                if (!task.IsCompleted)
                {
                    task.Wait();
                }

                //task.Wait();

                liveTimeCounter = cloud.liveTimeCounter;
                Tools.SetDirection(transform, cloud.direction);
                gameObject.transform.position = cloud.newPos;

                if (cloud.enabledImage == true && visible == true)
                {
                    image.enabled = true;
                }
                else
                {
                    image.enabled = false;
                }

                if (cloud.outOfBounds)
                {
                    StartCoroutine(FadeOut(0.1f));
                }
            }

            yield return null;
        }
    }

    //Seperate thread+
    private void Calculate(System.Action<Cloud,Cloud> result, Cloud _cloud)
    {
        float diffXA = _cloud.pos.x - cloud.posA.x;
        float diffXB = _cloud.pos.x - cloud.posB.x;

        _cloud.direction.x = _cloud.pos.x + (_cloud.tickSpeed * _cloud.speedModifier) * _cloud.deltaTime;

        _cloud.newPos = new Vector3(_cloud.pos.x + (_cloud.tickSpeed * _cloud.speedModifier) * _cloud.deltaTime, _cloud.pos.y, _cloud.zLevel);
        _cloud.liveTimeCounter = _cloud.liveTimeCounter + (_cloud.tickSpeed * _cloud.deltaTime);

        if (diffXA >= -_cloud.offsetDespawn && diffXA <= _cloud.offsetDespawn
            || diffXB >= -_cloud.offsetDespawn && diffXB <= _cloud.offsetDespawn
            || _cloud.liveTimeCounter >= _cloud.liveTime || _cloud.markedForDestroy || _cloud.speedModifier == 0f)
        {
            _cloud.outOfBounds = true;
        }

        _cloud.enabledImage = Tools.IsWithinCameraView(_cloud.occlusion);

        result(_cloud, _cloud);
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
