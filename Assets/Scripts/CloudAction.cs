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

public class CloudAction : MonoBehaviour
{
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

    private Cloud cloud;
    private CloudInstance cloudInstance;

    private void Start()
    {

        if (grid.rect.width > defaultWidth && influenceByGrid)
        {
            maxLiveTime *= (grid.rect.width / divideWidth);
        }

        liveTime = Random.Range(minLiveTime, maxLiveTime);

        cloudInstance = new CloudInstance(Result);
        cloud = new Cloud();

        StartCoroutine(FadeIn(0.1f));
    }

    private void Update()
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
            cloudInstance.Set(cloud);
            Task task = new Task(cloudInstance.Calculate);
            task.Start();
            task.Wait();

            liveTimeCounter = cloud.liveTimeCounter;
            Tools.SetDirection(transform, new Vector3(cloud.posX, 0f));
            gameObject.transform.position = cloud.newPos;
            if (cloud.outOfBounds)
            {
                StartCoroutine(FadeOut(0.1f));
            }
        }
    }

    //Seperate thread+
    private void Result(Cloud _cloud)
    {
        cloud = _cloud;
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
