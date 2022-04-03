/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, January 2019
 */
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CloudAction : BaseInfo
{
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
    public float liveTime;
    public Image image;
    public bool visible;
    public int tickCountMax;

    private void Start()
    {
        base.Initialize();
        CloudCycle.init.clouds.Add(tileId, this);

        if (grid.rect.width > defaultWidth && influenceByGrid)
        {
            maxLiveTime *= (grid.rect.width / divideWidth);
        }

        liveTime = Random.Range(minLiveTime, maxLiveTime);

        StartCoroutine(FadeIn(0.1f));
    }

    public void Move(CloudActionValues cloudActionValues)
    {
        if (Tick.init.speed == 0) return;

        liveTimeCounter = cloudActionValues.liveTimeCounter;
        Tools.SetDirection(transform, cloudActionValues.newPos);
        gameObject.transform.position = cloudActionValues.newPos;

        if (cloudActionValues.enabledImage == true && visible == true)
        {
            image.enabled = true;
        }
        else
        {
            image.enabled = false;
        }

        if (cloudActionValues.outOfBounds || markedForDestroy)
        {
            StartCoroutine(FadeOut(0.1f));
        }

    }

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
        if(cloudCycle.clouds.ContainsKey(tileId))
        {
            cloudCycle.clouds.Remove(tileId);
            cloudCycle.counter--;
        }
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
